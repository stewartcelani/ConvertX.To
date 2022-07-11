namespace ConvertX.To.Application.Interfaces;

public interface IUriService
{
    Uri GetFileUri(Guid conversionId);
}