using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SheetsProcessorApi.Data;
using SheetsProcessorApi.DTOs;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Services;

/// <summary>
/// Orchestrates statement extraction, enrichment, deduplication, and querying.
/// </summary>
public class StatementService : IStatementService
{
    private readonly IEnumerable<IStatementParserStrategy> _parserStrategies;
    private readonly ITransactionEnricher _enricher;
    private readonly AppDbContext _context;

    public StatementService(
        IEnumerable<IStatementParserStrategy> parserStrategies,
        ITransactionEnricher enricher,
        AppDbContext context)
    {
        _parserStrategies = parserStrategies;
        _enricher = enricher;
        _context = context;
    }

    public async Task<UploadResultDto> ProcessStatementAsync(IFormFile file)
    {
        ValidateFile(file);

        var extension = Path.GetExtension(file.FileName);
        var strategy = _parserStrategies.FirstOrDefault(s => s.CanHandle(extension));

        if (strategy == null)
            throw new NotSupportedException($"Formato não suportado: {extension}");

        var parsedTransactions = await strategy.ParseAsync(file);

        // Deduplicate within the uploaded file first
        var uniqueInFile = parsedTransactions
            .DistinctBy(t => t.Hash)
            .ToList();

        _enricher.EnrichTransactions(uniqueInFile);

        var hashes = uniqueInFile.Select(t => t.Hash).ToList();
        var existingHashes = await _context.Transactions
            .AsNoTracking()
            .Where(t => hashes.Contains(t.Hash))
            .Select(t => t.Hash)
            .ToHashSetAsync();

        var newTransactions = uniqueInFile
            .Where(t => !existingHashes.Contains(t.Hash))
            .ToList();

        if (newTransactions.Count > 0)
        {
            await _context.Transactions.AddRangeAsync(newTransactions);
            await _context.SaveChangesAsync();
        }

        var skippedCount = parsedTransactions.Count - newTransactions.Count;

        return new UploadResultDto(
            InsertedCount: newTransactions.Count,
            SkippedDuplicatesCount: skippedCount,
            Transactions: newTransactions
        );
    }

    public async Task<PagedResultDto<Transactions>> GetTransactionsAsync(
        string? category,
        bool? personalUse,
        string? search,
        int page = 1,
        int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = _context.Transactions.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(t => t.Category == category);
        }

        if (personalUse.HasValue)
        {
            query = query.Where(t => t.PersonalUse == personalUse.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(t =>
                t.Description.ToLower().Contains(term) ||
                t.Category.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenBy(t => t.Description)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResultDto<Transactions>(items, totalCount, page, pageSize, totalPages);
    }

    public async Task<StatementSummaryDto> GetSummaryAsync()
    {
        var categoryGroups = await _context.Transactions
            .AsNoTracking()
            .GroupBy(t => t.Category)
            .Select(g => new CategoryBreakdownDto(
                g.Key,
                g.Count(),
                g.Sum(t => t.Value)
            ))
            .OrderByDescending(c => c.Count)
            .ToListAsync();

        var totals = await _context.Transactions
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalCount = g.Count(),
                TotalRevenue = g.Where(t => t.Value > 0).Sum(t => t.Value),
                TotalExpenses = g.Where(t => t.Value < 0).Sum(t => t.Value),
                NetBalance = g.Sum(t => t.Value),
                TaxPaid = g.Where(t => t.Category == "Imposto MEI").Sum(t => Math.Abs(t.Value)),
                PersonalExpenses = g.Where(t => t.PersonalUse).Sum(t => Math.Abs(t.Value))
            })
            .FirstOrDefaultAsync();

        if (totals is null)
        {
            return new StatementSummaryDto(0, 0m, 0m, 0m, 0m, 0m, categoryGroups);
        }

        return new StatementSummaryDto(
            totals.TotalCount,
            totals.TotalRevenue,
            Math.Abs(totals.TotalExpenses),
            totals.NetBalance,
            totals.TaxPaid,
            totals.PersonalExpenses,
            categoryGroups
        );
    }

    public async Task<bool> DeleteTransactionAsync(Guid id)
    {
        var affected = await _context.Transactions
            .Where(t => t.Id == id)
            .ExecuteDeleteAsync();

        return affected > 0;
    }

    private static void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Nenhum arquivo enviado ou arquivo vazio.");
    }
}
