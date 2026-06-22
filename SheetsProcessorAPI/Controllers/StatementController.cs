using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using SheetsProcessorApi.Filters;
using SheetsProcessorApi.DTOs;
using SheetsProcessorApi.Interfaces;
using SheetsProcessorApi.Services;


namespace SheetsProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatementController : ControllerBase
{
    private readonly IStatementParserService _parserService;

    public StatementController(IStatementParserService parserService)
    {
        _parserService = parserService;
    }
    [HttpPost("upload")]
    [RequestSizeLimit(10_485_760)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    [AllowedExtensionsAttribute(new string []{ ".csv", ".xlsx" })]
    public async Task<IActionResult> UploadStatement(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
           return BadRequest(ApiResponse<string>.Fail(null, "Nenhum arquivo foi enviado ou o arquivo está vazio"));
        }

        string msg = "Uploado realizado com sucesso";

        return Accepted(new {Message = msg});
    }
}