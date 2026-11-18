// Arquivo: Services/XlsxParserStrategy.cs
using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;
using ClosedXML.Excel;

namespace SheetsProcessorApi.Services;

public class XlsxParserStrategy : IStatementParserStrategy
{
    public bool CanHandle(string extension) => extension.ToLower() == ".xlsx" || extension.ToLower() == ".xls";

    public async Task<List<Transactions>> ParseAsync(IFormFile file)
    {
        return await Task.Run(() =>
        {
            var transacoes = new List<Transactions>();

            using var stream = file.OpenReadStream();
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            var linhas = worksheet.RowsUsed().Skip(1);

            foreach (var linha in linhas)
            {
                var data = DateTime.SpecifyKind(linha.Cell(1).GetDateTime(), DateTimeKind.Utc);
                var valor = linha.Cell(2).GetValue<decimal>();
                var descricao = linha.Cell(3).GetString().Trim();

                transacoes.Add(new Transactions
                {
                    Date = data,
                    Value = valor,
                    Description = descricao,
                    Hash = $"{data:dd/MM/yyyy}_{valor}_{descricao}"
                });
            }
            return transacoes;
        });
    }
}