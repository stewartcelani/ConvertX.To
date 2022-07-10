using ConvertX.To.API.Entities;

namespace ConvertX.To.API.Services;

public interface IConversionService
{
    Task<Conversion> ConvertAsync(string targetFormat, IFormFile formFile);
    Task<Conversion> GetByIdAsync(Guid conversionId);
    Task<Stream> GetConvertedFileAsStreamAsync(Guid conversionId);
}