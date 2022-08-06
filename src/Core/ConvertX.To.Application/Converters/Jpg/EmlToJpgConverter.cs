using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Jpg;

public class EmlToJpgConverter : MsGraphDriveItemConverterBase
{
    public EmlToJpgConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}