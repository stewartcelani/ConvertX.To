using ConvertX.To.Application.Exceptions.Business;
using ConvertX.To.Application.Extensions;
using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public class ConverterFactory : IConverterFactory
{
    private readonly ILogger<ConverterFactory> _logger;
    private readonly IMsGraphFileConversionService _msGraphFileConversionService;

    public ConverterFactory(ILogger<ConverterFactory> logger, IMsGraphFileConversionService msGraphFileConversionService)
    {
        _logger = logger;
        _msGraphFileConversionService = msGraphFileConversionService;
    }

    public IConverter Create(string sourceFormat, string targetFormat)
    {
        try
        {
            var typeFullName = typeof(ConverterFactory).Namespace + "." + targetFormat.Proper() + "." + sourceFormat.Proper() + "To" + targetFormat.Proper() + "Converter";
            var type = Type.GetType(typeFullName);
            var constructorParams = new object[] { sourceFormat, targetFormat, _logger };
            if (type?.BaseType == typeof(MsGraphDriveItemConverterBase))
            {
                constructorParams = new object[] {  sourceFormat, targetFormat, _logger, _msGraphFileConversionService };
            }
            
            return (IConverter)Activator.CreateInstance(Type.GetType(typeFullName), constructorParams);
        }
        catch
        {
            throw new UnsupportedConversionException($"Converting from {sourceFormat.Proper()} to {targetFormat.Proper()} is not supported.");
        }
    }
}

