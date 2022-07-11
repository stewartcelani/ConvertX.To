using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public class DocxToPdfConverter : GraphConverter
{
    public DocxToPdfConverter(IGraphFileService graphFileService, ILogger logger) :
        base("docx", "pdf", graphFileService, logger)
    {
    }

    public override async Task<Stream> ConvertAsync(string filePath)
    {
        _logger.LogDebug($"{nameof(DocxToPdfConverter)}");
        return await base.ConvertAsync(filePath);
    }
}