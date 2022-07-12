using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters.Jpg;

public class M4vToJpgConverter : MsGraphDriveItemConverterBase
{
    public M4vToJpgConverter(string sourceFormat, string targetFormat, ILogger logger, IMsGraphFileConversionService msGraphFileConversionService) : base(sourceFormat, targetFormat, logger, msGraphFileConversionService)
    {
    }
}