using System.Transactions;
using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Services;

public class StatementParserFactory : IStatementParserFactory
{
    private readonly IEnumerable<IStatementParserStrategy> _strategies;
    public StatementParserFactory(IEnumerable<IStatementParserStrategy> strategies)
    {
        _strategies = strategies;
    }

    public async Task<List<Transactions>> ProcessFileAsync(IFormFile file, string extension)
    {
        var strategy = _strategies.FirstOrDefault(s=> s.CanHandle(extension));

        if (strategy == null)
            throw new NotSupportedException($"Formato não suportado: {extension}");

        return await strategy.ParseAsync(file);
    }

}