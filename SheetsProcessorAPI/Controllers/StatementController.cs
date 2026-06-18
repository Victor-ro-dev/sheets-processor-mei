using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SheetsProcessorApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatementController : ControllerBase
{
    [HttpPost("upload")]
    [RequestSizeLimit(10_485_760)]
    [RequestFormLimits(MultipartBodyLengthLimit = 10485760)]
    public async Task<IActionResult> UploadStatement(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
           return BadRequest(new {Message = "Nenhum arquivo foi enviado ou o arquivo está vazio"});
        }

        string msg = "Uploado realizado com suecesso";

        return Accepted(new {Message = msg});
    }
}