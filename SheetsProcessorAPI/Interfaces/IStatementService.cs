using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Interfaces;

public interface IStatementService
{
    Task<List<Transactions>> ProcessStatementAsync(IFormFile file);
}
