using System.Diagnostics.CodeAnalysis;
using ConvertX.To.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Middleware;

public class ApiExceptionMiddleware
{
    private readonly RequestDelegate _request;

    public ApiExceptionMiddleware(RequestDelegate request)
    {
        _request = request;
    }

    [ExcludeFromCodeCoverage]
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _request(context);
        }
        catch (ApiException exception)
        {
            context.Response.StatusCode = 500;

            var error = new ValidationProblemDetails
            {
                Title = "One or more errors occurred.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Status = 500,
                Extensions =
                {
                    ["traceId"] = context.TraceIdentifier
                }
            };
            foreach (var validationFailure in exception.Errors)
                error.Errors.Add(new KeyValuePair<string, string[]>(
                    validationFailure.PropertyName,
                    new[] { validationFailure.ErrorMessage }));
            await context.Response.WriteAsJsonAsync(error);
        }
    }
}