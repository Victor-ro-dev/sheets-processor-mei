using System.Transactions;
using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Models;


namespace SheetsProcessorApi.Interfaces;

public interface IStatementParserStrategy
{
    bool CanHandle(string extension);
    Task<List<Transactions>> ParseAsync(IFormFile file);
    
}