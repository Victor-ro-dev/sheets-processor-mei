using Microsoft.AspNetCore.Http;

namespace SheetsProcessorApi.Interfaces;

public interface IStatementParserService
{
    Task<List<string>> ParseAsync(IFormFile file);
}