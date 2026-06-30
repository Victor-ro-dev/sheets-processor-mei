using System.Globalization;
using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Services;

public class CsvParserStrategy : IStatementParserStrategy
{
    public bool CanHandle(string extension)
    {
        return extension.ToLower() == ".csv";
    }

    public async Task<List<Transactions>> ParseAsync(IFormFile file)
    {
        var transactions = new List<Transactions>();

        using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);

        await reader.ReadLineAsync();

        string? line;

        while((line = await reader.ReadLineAsync())is not null)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            var columns = line.Split(";");

            if (columns.Length >=3)
            {
                transactions.Add(new Transactions
                {
                    Date = DateTime.ParseExact(columns[0], "dd/MM/yyyy", CultureInfo.InstalledUICulture),
                    Value = decimal.Parse(columns[1], CultureInfo.InvariantCulture),
                    Description = columns[2],
                    Hash = $"{columns[0]}_{columns[1]}_{columns[2]}"
                });
            }
        }
        return transactions;
    }
}