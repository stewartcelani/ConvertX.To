using ConvertX.To.Application.Converters;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionEngine
{
    Task<Stream> ConvertAsync(string sourceFormat, string targetFormat, Stream source);
    SupportedConversions GetSupportedConversions();
}