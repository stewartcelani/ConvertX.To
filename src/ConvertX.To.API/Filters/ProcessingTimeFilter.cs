using Microsoft.AspNetCore.Mvc.Filters;

namespace ConvertX.To.API.Filters;

public class ProcessingTimeFilter : IAsyncActionFilter
{
    private const string ProcessingTimeHeaderName = "x-processing-time";

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // TODO: This is showing 50ms when a DocxToPdf file conversion took 7.2s via Postman, fix
        var requestStart = DateTime.Now;
        await next();
        var timeElapsed = DateTime.Now - requestStart;
        context.HttpContext.Response.Headers.Add(ProcessingTimeHeaderName, $"{timeElapsed.Milliseconds.ToString()}ms");
    }
}