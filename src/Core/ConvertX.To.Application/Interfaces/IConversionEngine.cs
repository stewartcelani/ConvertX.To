using ConvertX.To.Application.Converters;
using ConvertX.To.Domain.Common;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionEngine
{
    Task<(string, Stream)> ConvertAsync(string sourceFormat, string targetFormat, Stream source,
        ConversionOptions conversionOptions);
    SupportedConversions GetSupportedConversions();
}