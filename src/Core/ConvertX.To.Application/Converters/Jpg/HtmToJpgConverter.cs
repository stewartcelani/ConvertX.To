using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Jpg;

public class HtmToJpgConverter : MsGraphDriveItemConverterBase
{
    public HtmToJpgConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}