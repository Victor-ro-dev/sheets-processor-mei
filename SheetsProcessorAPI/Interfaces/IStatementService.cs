using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.DTOs;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Interfaces;

public interface IStatementService
{
    Task<UploadResultDto> ProcessStatementAsync(IFormFile file);
    Task<PagedResultDto<Transactions>> GetTransactionsAsync(
        string? category,
        bool? personalUse,
        string? search,
        int page = 1,
        int pageSize = 20);
    Task<StatementSummaryDto> GetSummaryAsync();
    Task<bool> DeleteTransactionAsync(Guid id);
}
