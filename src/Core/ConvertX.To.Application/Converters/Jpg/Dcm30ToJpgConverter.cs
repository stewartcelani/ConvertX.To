using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Jpg;

public class Dcm30ToJpgConverter : MsGraphDriveItemConverterBase
{
    public Dcm30ToJpgConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}