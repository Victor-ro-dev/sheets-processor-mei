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
    private readonly IStatementParserFactory _parserFactory;
    private readonly ITransactionEnricher _enricher;

    // A mágica da Injeção de Dependência acontece aqui
    public StatementController(
        IStatementParserFactory parserFactory, 
        ITransactionEnricher enricher)
    {
        _parserFactory = parserFactory;
        _enricher = enricher;
    }

    [HttpPost("upload")]
    [AllowedExtensions(new string[] { ".csv", ".xlsx" })]
    public async Task<IActionResult> UploadStatement(IFormFile file)
    {
        // Validação básica de segurança (evita arquivos vazios)
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<string>.Fail(null, "Nenhum arquivo enviado ou arquivo vazio."));
        }

        // Pega a extensão (.csv ou .xlsx) para o roteamento
        var extension = Path.GetExtension(file.FileName);

        try 
        {
            // PASSO 1: Extração (I/O Bound - Assíncrono)
            // A Thread é liberada enquanto o disco/rede lê o arquivo
            List<Transactions> transactions = await _parserFactory.ProcessFileAsync(file, extension);

            // PASSO 2: Transformação (CPU Bound - Síncrono)
            // Processamento na memória RAM em nanossegundos
            _enricher.EnrichTransactions(transactions);

            // PASSO 3: Resposta Padronizada
            var msg = $"Processado com sucesso. {transactions.Count} transações categorizadas.";
            return Ok(ApiResponse<List<Transactions>>.Ok(transactions, msg));
        }
        catch (NotSupportedException ex)
        {
            // Se escapar do Decorator, o throw da Factory é capturado aqui
            return BadRequest(ApiResponse<string>.Fail(null, ex.Message));
        }
        catch (Exception ex)
        {
            // Rede de segurança para arquivos corrompidos (ex: data em formato errado)
            return StatusCode(500, ApiResponse<string>.Fail(null, $"Erro interno ao processar o arquivo: {ex.Message}"));
        }
    }
}