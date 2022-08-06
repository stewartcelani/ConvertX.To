using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Application.Converters.Pdf;

public class PptToPdfConverter : MsGraphDriveItemConverterBase
{
    public PptToPdfConverter(string sourceFormat, string targetFormat, ILoggerAdapter<IConverter> logger,
        IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger,
        msGraphFileConversionService)
    {
    }
}