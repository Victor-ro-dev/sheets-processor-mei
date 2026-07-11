// Arquivo: Data/AppDbContext.cs
using Microsoft.EntityFrameworkCore;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transactions> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Garantia de Hash Único (Evita duplicidade de extrato)
        modelBuilder.Entity<Transactions>()
            .HasIndex(t => t.Hash)
            .IsUnique();

        // Índice para buscas de relatórios mensais do MEI
        modelBuilder.Entity<Transactions>()
            .HasIndex(t => t.Date);
    }
}