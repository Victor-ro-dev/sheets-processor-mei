using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AllowerdExtensionsAttribute : ActionFilterAttribute
{
    private readonly string[] _extension;

    public AllowerdExtensionsAttribute(string[] extensions)
    {
        _extension = extensions;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
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