using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Jpg;

public class PptmToJpgConverter : ToJpgConverterBase
{
    public PptmToJpgConverter(ConverterFactory converterFactory, string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(converterFactory, sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}