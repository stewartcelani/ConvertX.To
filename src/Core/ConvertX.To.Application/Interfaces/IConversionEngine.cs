using ConvertX.To.Application.Converters;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionEngine
{
    Task<(string, Stream)> ConvertAsync(string sourceFormat, string targetFormat, Stream source,
        ConversionOptions conversionOptions);
    SupportedConversions GetSupportedConversions();
}