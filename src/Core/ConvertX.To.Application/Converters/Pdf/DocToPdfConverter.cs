using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Pdf;

public class DocToPdfConverter : MsGraphDriveItemConverterBase
{
    public DocToPdfConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}