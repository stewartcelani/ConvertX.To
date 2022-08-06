using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Jpg;

public class PotToJpgConverter : MsGraphDriveItemConverterBase
{
    public PotToJpgConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}