using System.Net;
using System.Text.Json;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Exceptions;
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
        httpResponse.StatusCode = MapExceptionToStatusCode(exception);

        if (exception is HttpResponseException httpResponseException)
        {
            exceptionType = exception.GetType().Name;
            message = exception.Message;
            httpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = httpResponseException.HttpResponseMessage.ReasonPhrase;
        }

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
        if (exception is HttpResponseException httpResponseException)
        {
            return (int)httpResponseException.HttpResponseMessage.StatusCode;
        }
        
        return exception switch
        {
            UnsupportedConversionException => 415,
            ConversionNotFoundException => 404,
            _ => 500
        };
    }
}