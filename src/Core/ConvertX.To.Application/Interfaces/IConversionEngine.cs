using ConvertX.To.Application.Domain;
using ConvertX.To.Domain.Options;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionEngine
{
    Task<(string, Stream)> ConvertAsync(string sourceFormat, string targetFormat, Stream source,
        ConversionOptions conversionOptions);

    //SupportedConversions GetSupportedConversions(); // moved this to static
}