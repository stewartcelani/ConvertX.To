using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain;

namespace ConvertX.To.ConsoleUI.API.Services;

public interface IConversionService
{
    Task<SupportedConversions> GetSupportedConversionsAsync();
    Task ConvertFileToAllSupportedFormatsAsync(FileInfo file, SupportedConversions supportedConversions);
    Task ConvertFileToTargetFormatAsync(FileInfo file, string targetFormat);
}