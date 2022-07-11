using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Jpg;

public class DocxToJpgConverter : MsGraphDriveItemConverterBase
{
    public DocxToJpgConverter(IMsGraphFileConversionService msGraphFileConversionService, ILogger logger) : base("jpg", msGraphFileConversionService, logger)
    {
    }
}