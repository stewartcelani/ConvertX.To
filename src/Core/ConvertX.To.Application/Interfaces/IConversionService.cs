using ConvertX.To.Application.Converters;
using ConvertX.To.Domain.Entities;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionService
{
    Task<Conversion> ConvertAsync(string targetFormat, string fileName, Stream stream);
    Task<Conversion> GetByIdAsync(Guid conversionId);
    Task<Stream> DownloadFileAsync(Guid conversionId);
    SupportedConversions GetSupportedConversions();
}