using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Pdf;

public class PpsxToPdfConverter : MsGraphDriveItemConverterBase
{
    public PpsxToPdfConverter(string sourceFormat, string targetFormat, ILogger logger, IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger, msGraphFileConversionService)
    {
    }
}