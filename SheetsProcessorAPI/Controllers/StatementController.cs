// Arquivo: Controllers/StatementController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.DTOs;
using SheetsProcessorApi.Models;
using SheetsProcessorApi.Filters;

namespace SheetsProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")] // Rota base: /api/statement
public class StatementController : ControllerBase
{
    private readonly IStatementService _statementService;

    /// <summary>
    /// Controller recebe APENAS o Serviço de Orquestração.
    /// Não conhece Factory, Enricher, Parser ou lógica de BD.
    /// </summary>
    public StatementController(IStatementService statementService)
    {
        _statementService = statementService;
    }

    [HttpPost("upload")]
    [AllowedExtensions(new string[] { ".csv", ".xlsx" })]
    public async Task<IActionResult> UploadStatement(IFormFile file)
    {
        try
        {
            // Delega TODO o processamento para o serviço
            // Controller é apenas um "gatilho" HTTP
            var transactions = await _statementService.ProcessStatementAsync(file);

            var msg = $"Processado com sucesso. {transactions.Count} transações categorizadas.";
            return Ok(ApiResponse<List<Transactions>>.Ok(transactions, msg));
        }
        catch (NotSupportedException ex)
        {
            // Extensão não suportada
            return BadRequest(ApiResponse<string>.Fail(null, ex.Message));
        }
        catch (ArgumentException ex)
        {
            // Validação de arquivo
            return BadRequest(ApiResponse<string>.Fail(null, ex.Message));
        }
        catch (Exception ex)
        {
            // Rede de segurança para arquivos corrompidos (ex: data em formato errado)
            return StatusCode(500, ApiResponse<string>.Fail(null, $"Erro interno ao processar o arquivo: {ex.Message}"));
        }
    }
}