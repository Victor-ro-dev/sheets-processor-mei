using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SheetsProcessorApi.Filters;

public class AllowedExtensionsAttribute : ActionFilterAttribute
{
    private readonly string[] _extension;

    public AllowedExtensionsAttribute(string[] extensions) // Argumentos que são colocados dentro do decorador do controller: [AllowedExtensionsAttribute(new string []{ ".csv", ".xlsx" })]
    {
        _extension = extensions;
    }

    public override void OnActionExecuting(ActionExecutingContext context) // Contexto que vem da req: 
    // public async Task<IActionResult> UploadStatement(IFormFile file)
    {
        var file = context.ActionArguments.Values.OfType<IFormFile>().FirstOrDefault();

        if (file != null)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_extension.Contains(extension))
            {
                context.Result = new UnsupportedMediaTypeResult();
            }
        }
    }
}