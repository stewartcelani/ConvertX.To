using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Pdf;

public class OdtToPdfConverter : MsGraphDriveItemConverterBase
{
    public OdtToPdfConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}