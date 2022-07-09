using ConvertX.To.API.Settings;

namespace ConvertX.To.API.Converters;

public class DocxToPdfConverter : AzureConverter
{
    private const string _sourceFormat = "docx";
    private const string _targetFormat = "pdf";
    
    public DocxToPdfConverter(AzureSettings azureSettings, ILogger logger) : base(_sourceFormat, _targetFormat, azureSettings, logger)
    {
    }
    
    public override async Task<FileInfo> ConvertAsync(FileInfo file)
    {
        _logger.LogDebug($"{nameof(DocxToPdfConverter)}");
        return await base.ConvertAsync(file);
    }
}