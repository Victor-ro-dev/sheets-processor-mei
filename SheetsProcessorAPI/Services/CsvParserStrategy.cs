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

            if (columns.Length >= 3)
            {
                var dateText = columns[0].Trim();
                var valueText = columns[1].Trim();
                var description = columns[2].Trim();

                var parsedDate = DateTime.ParseExact(dateText, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var utcDate = DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc);
                var parsedValue = decimal.Parse(valueText, CultureInfo.InvariantCulture);

                transactions.Add(new Transactions
                {
                    Date = utcDate,
                    Value = parsedValue,
                    Description = description,
                    Hash = $"{dateText}_{parsedValue.ToString(CultureInfo.InvariantCulture)}_{description}"
                });
            }
        }
        return transactions;
    }
}