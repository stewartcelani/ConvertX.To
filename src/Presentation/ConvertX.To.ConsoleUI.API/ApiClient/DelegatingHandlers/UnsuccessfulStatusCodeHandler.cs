using System.Net;
using ConvertX.To.ConsoleUI.API.Exceptions;
using Refit;

namespace ConvertX.To.ConsoleUI.API.ApiClient.DelegatingHandlers;

public class UnsuccessfulStatusCodeHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        
        if (!response.IsSuccessStatusCode/* || string.IsNullOrWhiteSpace(response.Content.ToString())*/)
        {
            throw new UnsuccessfulStatusCodeException(response);
        }

        return response;
    }

    
}