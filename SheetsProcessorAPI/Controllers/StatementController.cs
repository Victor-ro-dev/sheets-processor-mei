using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SheetsProcessorApi.DTOs;
using SheetsProcessorApi.Filters;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Models;

namespace SheetsProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatementController : ControllerBase
{
    private readonly IStatementService _statementService;

    public StatementController(IStatementService statementService)
    {
        _statementService = statementService;
    }

    [HttpPost("upload")]
    [AllowedExtensions(new[] { ".csv", ".xlsx" })]
    public async Task<IActionResult> UploadStatement(IFormFile file)
    {
        try
        {
            var result = await _statementService.ProcessStatementAsync(file);

            var msg = result.SkippedDuplicatesCount > 0
                ? $"Processado com sucesso. {result.InsertedCount} novas transações salvas ({result.SkippedDuplicatesCount} duplicadas ignoradas)."
                : $"Processado com sucesso. {result.InsertedCount} transações categorizadas.";

            return Ok(ApiResponse<UploadResultDto>.Ok(result, msg));
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(null, ex.Message));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(null, ex.Message));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<string>.Fail(null, $"Erro interno ao processar o arquivo: {ex.Message}"));
        }
    }

    [HttpGet("transactions")]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] string? category,
        [FromQuery] bool? personalUse,
        [FromQuery] string? search,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var result = await _statementService.GetTransactionsAsync(category, personalUse, search, page, pageSize);
        return Ok(ApiResponse<PagedResultDto<Transactions>>.Ok(result));
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await _statementService.GetSummaryAsync();
        return Ok(ApiResponse<StatementSummaryDto>.Ok(summary));
    }

    [HttpDelete("transactions/{id:guid}")]
    public async Task<IActionResult> DeleteTransaction(Guid id)
    {
        var deleted = await _statementService.DeleteTransactionAsync(id);
        if (!deleted)
            return NotFound(ApiResponse<string>.Fail(null, "Transação não encontrada."));

        return Ok(ApiResponse<string>.Ok("Removida", "Transação removida com sucesso."));
    }
}
