using System.Transactions;
using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Interfaces;

public interface IStatementParserFactory
{
    Task<List<Transaction>> ProcessFileAsync(IFormFile file, string extension);
}