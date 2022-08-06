using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Pdf;

public class HtmlToPdfConverter : MsGraphDriveItemConverterBase
{
    public HtmlToPdfConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}