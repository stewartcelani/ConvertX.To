using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Extensions;
using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters;

public class ConverterFactory : IConverterFactory
{
    private readonly ILoggerAdapter<IConverter> _converterLogger;
    private readonly ILoggerAdapter<ConverterFactory> _logger;
    private readonly IMsGraphFileConversionService _msGraphFileConversionService;

    public ConverterFactory(ILoggerAdapter<ConverterFactory> logger,
        ILoggerAdapter<IConverter> converterLogger,
        IMsGraphFileConversionService msGraphFileConversionService)
    {
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _converterLogger = converterLogger ?? throw new NullReferenceException(nameof(converterLogger));
        _msGraphFileConversionService = msGraphFileConversionService ??
                                        throw new NullReferenceException(nameof(msGraphFileConversionService));
    }

    public IConverter Create(string sourceFormat, string targetFormat)
    {
        var sourceFormatProper = Proper(sourceFormat);
        var targetFormatProper = Proper(targetFormat);
        try
        {
            var typeFullName = typeof(ConverterFactory).Namespace + "." + targetFormatProper + "." +
                               sourceFormatProper + "To" + targetFormatProper + "Converter";
            
            var type = Type.GetType(typeFullName);
            
            var constructorParams = new object[] { sourceFormat, targetFormat, _converterLogger };
            
            if (type?.BaseType == typeof(ToJpgConverterBase))
                constructorParams = new object[]
                    { this, sourceFormat, targetFormat, _converterLogger, _msGraphFileConversionService };
            
            if (type?.BaseType == typeof(MsGraphDriveItemConverterBase))
                constructorParams = new object[]
                    { sourceFormat, targetFormat, _converterLogger, _msGraphFileConversionService };

            return (IConverter)Activator.CreateInstance(Type.GetType(typeFullName), constructorParams);
        }
        catch
        {
            throw new UnsupportedConversionException(
                $"Converting from {sourceFormatProper} to {targetFormatProper} is not supported.");
        }
    }

    private string Proper(string s)
    {
        return s.First().ToString().ToUpper() + s.ToLower()[1..];
    }
}