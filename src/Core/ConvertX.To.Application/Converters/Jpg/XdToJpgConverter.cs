using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Jpg;

public class XdToJpgConverter : MsGraphDriveItemConverterBase
{
    public XdToJpgConverter(string sourceFormat, string targetFormat, ILogger logger, IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger, msGraphFileConversionService)
    {
    }
}