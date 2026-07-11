using Microsoft.EntityFrameworkCore;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Services;
// using SheetsProcessorApi.Data; // Substitua pela pasta onde criar seu AppDbContext

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// ====================================================================
// CONFIGURAÇÃO DO BANCO DE DADOS (POSTGRESQL)
// ====================================================================
// 1. Buscamos a string de conexão de dentro do appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Registramos o DbContext usando o motor do PostgreSQL
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql(connectionString));

// ====================================================================
// INJEÇÃO DE DEPENDÊNCIAS - ORQUESTRAÇÃO DO PIPELINE
// ====================================================================
// Strategies (implementações de parsing)
builder.Services.AddScoped<IStatementParserStrategy, CsvParserStrategy>();
builder.Services.AddScoped<IStatementParserStrategy, XlsxParserStrategy>();

// Enricher (lógica de negócio)
builder.Services.AddScoped<ITransactionEnricher, TransactionEnricher>();

// Serviço de orquestração do pipeline (o que o Controller chama)
builder.Services.AddScoped<IStatementService, StatementService>();

var app = builder.Build();
app.MapControllers();
app.Run();