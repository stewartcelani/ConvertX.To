using System.Text.Json;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.API.Exceptions;
using Microsoft.AspNetCore.Http.Features;

namespace ConvertX.To.API.Middleware.Translators;

public static class ExceptionToHttpTranslator
{
    public static async Task Translate(HttpContext httpContext, Exception exception)
    {
        var httpResponse = httpContext.Response;
        
        var exceptionType = "Exception";
        var message = "Internal Server Error";

        if (exception is ConvertXToPublicException publicException)
        {
            exceptionType = exception.GetType().Name;
            message = exception.Message;
            httpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = publicException.Reason;
        }

        httpResponse.StatusCode = MapExceptionToStatusCode(exception);
        httpResponse.Headers.Add("Exception-Type", exceptionType);

        var errorResponse = new ErrorResponse
        {
            Errors = new List<ErrorModel>
            {
                new()
                {
                    FieldName = exceptionType,
                    Message = message
                }
            }
        };

        httpContext.Response.ContentType = "application/json";
        var jsonErrorResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        await httpResponse.WriteAsync(jsonErrorResponse);
    }

    private static int MapExceptionToStatusCode(Exception exception)
    {
        if (exception is UnsupportedConversionException)
        {
            return 400;
        }

        return 500;
    }
}