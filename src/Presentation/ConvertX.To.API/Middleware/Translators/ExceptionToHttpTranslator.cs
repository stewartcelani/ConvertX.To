using System.Text.Json;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Exceptions.Business;
using Microsoft.AspNetCore.Http.Features;

namespace ConvertX.To.API.Middleware.Translators;

public static class ExceptionToHttpTranslator
{
    public static async Task Translate(HttpContext httpContext, Exception exception)
    {
        var httpResponse = httpContext.Response;
        httpContext.Response.ContentType = "application/json";
        
        var exceptionType = "Exception";
        var message = "Internal Server Error";

        if (exception is ConvertXToBusinessExceptionBase businessException)
        {
            exceptionType = exception.GetType().Name;
            message = exception.Message;
            httpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = businessException.Reason;
        }

        httpResponse.StatusCode = MapExceptionToStatusCode(exception);
        httpResponse.Headers.Add("Exception-Type", exceptionType);

        var errorResponse = new ErrorResponse
        {
            Errors = new List<ErrorResponseModel>
            {
                new()
                {
                    Error = exceptionType,
                    Message = message
                }
            }
        };

        var jsonErrorResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        await httpResponse.WriteAsync(jsonErrorResponse);
    }

    private static int MapExceptionToStatusCode(Exception exception)
    {
        return exception switch
        {
            UnsupportedConversionException => 415,
            ConversionNotFoundException => 404,
            ConvertXToBusinessExceptionBase => 400,
            _ => 500
        };
    }
}