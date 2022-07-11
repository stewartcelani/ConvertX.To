using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public class DocxToPdfConverter : MsGraphDriveItemConverterBase
{
    public DocxToPdfConverter(IMsGraphFileConversionService msGraphFileConversionService, ILogger logger) :
        base("pdf", msGraphFileConversionService, logger)
    {
    }
    
}