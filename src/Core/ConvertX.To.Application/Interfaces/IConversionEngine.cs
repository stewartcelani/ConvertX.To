using ConvertX.To.Application.Converters;

namespace ConvertX.To.Application.Interfaces;

public interface IConversionEngine
{
    Task<ConversionResult> ConvertAsync(ConversionTask task);
}