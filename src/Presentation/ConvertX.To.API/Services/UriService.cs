using ConvertX.To.API.Contracts.V1;
using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.API.Services;

public class UriService : IUriService
{
    private readonly string _baseUri;

    public UriService(string baseUri)
    {
        _baseUri = baseUri;
    }

    public Uri GetFileUri(Guid conversionId)
    {
        return new Uri(_baseUri + ApiRoutesV1.Files.Get.Replace("{conversionId}", conversionId.ToString()));
    }
}