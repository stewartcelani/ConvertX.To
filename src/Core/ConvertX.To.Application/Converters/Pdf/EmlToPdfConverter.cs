using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Pdf;

public class EmlToPdfConverter : MsGraphDriveItemConverterBase
{
    public EmlToPdfConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}