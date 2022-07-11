using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Pdf;

public class HtmlToPdfConverter : MsGraphDriveItemConverterBase
{
    public HtmlToPdfConverter(string sourceFormat, string targetFormat, ILogger logger, IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger, msGraphFileConversionService)
    {
    }
}