using ConvertX.To.API.Services;

namespace ConvertX.To.API.Converters;

public class DocxToPdfConverter : AzureConverter
{
    public DocxToPdfConverter(IAzureFileService azureFileService, ILogger logger) :
        base("docx", "pdf", azureFileService, logger)
    {
    }

    public override async Task<Stream> ConvertAsync(string filePath)
    {
        _logger.LogDebug($"{nameof(DocxToPdfConverter)}");
        return await base.ConvertAsync(filePath);
    }
}