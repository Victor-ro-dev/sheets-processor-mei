using Microsoft.AspNetCore.Http;

namespace SheetsProcessorApi.Interfaces;

public interface IStatementsParseService
{
    Task<List<string>> ParseAsync(IFormFile file);
}