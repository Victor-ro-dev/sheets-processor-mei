using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;
using SheetsProcessorApi.Data;

namespace SheetsProcessorApi.Services;

/// <summary>
/// Implementação do serviço de orquestração do pipeline.
/// 
/// Responsabilidades:
/// 1. Validar extensão do arquivo
/// 2. Chamar Factory para parsear (CSV/XLSX)
/// 3. Chamar Enricher para categorizar
/// 4. Retornar resultado processado
/// 
/// O Controller não sabe detalhes de como tudo isso funciona.
/// </summary>
public class StatementService : IStatementService
{
    private readonly IEnumerable<IStatementParserStrategy> _parserStrategy;
    private readonly ITransactionEnricher _enricher;
    private readonly AppDbContext _context;

    public StatementService(
        IEnumerable<IStatementParserStrategy> parserStrategy,
        ITransactionEnricher enricher,
        AppDbContext context)
    {
        _parserStrategy = parserStrategy;
        _enricher = enricher;
        _context = context;
    }

    public async Task<List<Transactions>> ProcessStatementAsync(IFormFile file)
    {
        // Validações básicas
        ValidateFile(file);

        // Pega extensão
        var extension = Path.GetExtension(file.FileName);

        var strategy = _parserStrategy.FirstOrDefault(s => s.CanHandle(extension));
        
        if (strategy == null)
            throw new NotSupportedException($"Formato não suportado: {extension}");

        // 1. Extração
        var transactions = await strategy.ParseAsync(file);


        // PASSO 2: Enriquecimento (categorização de negócio)
        _enricher.EnrichTransactions(transactions);

        // 3. Carga / Persistência
        await _context.Transactions.AddRangeAsync(transactions);
        await _context.SaveChangesAsync();

        // PASSO 3: Retorna resultado
        return transactions;
    }

    private void ValidateFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Nenhum arquivo enviado ou arquivo vazio.");
    }
}
