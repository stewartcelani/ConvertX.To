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

        var statusCode = MapExceptionToHttpStatusCode(exception);
        
        httpResponse.StatusCode = (int)statusCode;

        if (statusCode is HttpStatusCode.InternalServerError) await httpResponse.CompleteAsync();

        var errorResponse = MapExceptionToErrorResponse(exception);

        httpContext.Response.ContentType = "application/json";
        
        var jsonErrorResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        await httpResponse.WriteAsync(jsonErrorResponse);
    }
    
    private static HttpStatusCode MapExceptionToHttpStatusCode(Exception exception)
    {
        if (exception is not IBusinessException) return HttpStatusCode.InternalServerError;

        return exception switch
        {
            UnsupportedConversionException => HttpStatusCode.UnsupportedMediaType,
            ConversionNotFoundException => HttpStatusCode.NotFound,
            ConvertedFileGoneException => HttpStatusCode.Gone,
            InvalidFileLengthException => HttpStatusCode.BadRequest,
            _ => throw new ArgumentOutOfRangeException(nameof(exception))
        };
    }

    private static ErrorResponse MapExceptionToErrorResponse(Exception exception)
    {
        return new ErrorResponse
        {
            Errors = new List<ErrorResponseModel>()
            {
                new (exception.GetType().Name, exception.Message)
            }
        };
    }
    
}