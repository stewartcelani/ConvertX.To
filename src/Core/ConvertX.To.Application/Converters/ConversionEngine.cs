using System.Reflection;
using ConvertX.To.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Application.Converters;

public class ConversionEngine : IConversionEngine
{
    private readonly ILogger<ConversionEngine> _logger;
    private readonly IConverterFactory _converterFactory;

    public ConversionEngine(ILogger<ConversionEngine> logger,
        IConverterFactory converterFactory)
    {
        _logger = logger;
        _converterFactory = converterFactory;
    }

    // TODO: To improve the Jpg converters (details below):
    // 1. If the sourceFormat has a {sourceFormat}ToPdfConverter use that to convert to pdf
    // 2. Split the PDF using PdfSharp
    // 3. Loop through split pages and convert each to Jpg
    // 4. Zip them and store the zip file as the associated file
    // http://www.pdfsharp.com/PDFsharp/index.php?option=com_content&task=view&id=37&Itemid=48
    // TODO: If can use PdfSharp to convert from Jpg to Pdf then can transparently convert all the {sourceFormat}ToJpg formats to Pdf as well
    // TODO: Look into QuestPdf: https://www.questpdf.com/documentation/api-reference.html#static-images
    // Could try use it to convert Jpg to Pdf and to also write .txt, .log, .json (text formats) to PDF which can then be converted into JPG

    public async Task<Stream> ConvertAsync(string sourceFormat, string targetFormat, Stream source)
    {
        _logger.LogInformation(
            $"Processing new request to convert {sourceFormat} to {targetFormat}");

        var converter = _converterFactory.Create(sourceFormat, targetFormat);

        return await converter.ConvertAsync(source);
    }
    
}