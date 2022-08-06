namespace ConvertX.To.API.Services;

public interface IUriService
{
    Uri GetFileUri(Guid conversionId);
}