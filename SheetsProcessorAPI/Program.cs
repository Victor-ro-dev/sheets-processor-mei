using Microsoft.EntityFrameworkCore;
using Npgsql;
using SheetsProcessorApi.Data;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Services;

var builder = WebApplication.CreateBuilder(args);

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

builder.Services.AddControllers();

var connectionString = ResolvePostgresConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!string.IsNullOrWhiteSpace(connectionString))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        var sqlitePath = Path.Combine(Path.GetTempPath(), "sheets_processor.db");
        options.UseSqlite($"Data Source={sqlitePath}");
    }
});

var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        }
    });
});

builder.Services.AddScoped<IStatementParserStrategy, CsvParserStrategy>();
builder.Services.AddScoped<IStatementParserStrategy, XlsxParserStrategy>();
builder.Services.AddScoped<ITransactionEnricher, TransactionEnricher>();
builder.Services.AddScoped<IStatementService, StatementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("FrontendPolicy");
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

static string? ResolvePostgresConnectionString(IConfiguration configuration)
{
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    if (!string.IsNullOrWhiteSpace(databaseUrl) &&
        (databaseUrl.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
         databaseUrl.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase)))
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':', 2);
        var connBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty,
            SslMode = SslMode.Require
        };
        return connBuilder.ConnectionString;
    }

    return configuration.GetConnectionString("DefaultConnection");
}
