using System.Diagnostics;
using  Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Interfaces;

namespace SheetsProcessorApi.Services;

public class CsvStatementParserService : IStatementParserService
{
    public async Task<List<string>> ParseAsync(IFormFile file)
    {
        var proccessedLines = new List<string>();

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);

        string? line;

        while ((line = await reader.ReadLineAsync()) is not null)
        {
            
            if (!string.IsNullOrWhiteSpace(line))
            {
                proccessedLines.Add(line);
            }
        }

        return proccessedLines;
    }
}
