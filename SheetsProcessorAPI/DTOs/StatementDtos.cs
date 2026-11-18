namespace SheetsProcessorApi.DTOs;

public sealed record CategoryBreakdownDto(
    string Category,
    int Count,
    decimal TotalValue
);

public sealed record StatementSummaryDto(
    int TotalCount,
    decimal TotalRevenue,
    decimal TotalExpenses,
    decimal NetBalance,
    decimal TaxPaid,
    decimal PersonalExpenses,
    List<CategoryBreakdownDto> Categories
);

public sealed record PagedResultDto<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages
);

public sealed record UploadResultDto(
    int InsertedCount,
    int SkippedDuplicatesCount,
    List<SheetsProcessorApi.Models.Transactions> Transactions
);
