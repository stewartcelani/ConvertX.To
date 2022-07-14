using System.Reflection;
using System.Runtime.CompilerServices;
using ConvertX.To.Application.Helpers;
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

    public async Task<(string, Stream)> ConvertAsync(string sourceFormat, string targetFormat, Stream source,
        ConversionOptions conversionOptions)
    {
        _logger.LogInformation(
            "Processing new request to convert {sourceFormat} to {targetFormat}", sourceFormat, targetFormat);

        var converter = _converterFactory.Create(sourceFormat, targetFormat);

        return await converter.ConvertAsync(source, conversionOptions);
    }

    public SupportedConversions GetSupportedConversions()
    {
        var converters = Reflection.GetConcreteTypesInAssembly<IConverter>(Assembly.GetExecutingAssembly());

        var convertersByTargetFormat = new Dictionary<string, List<string>>();
        var convertersBySourceFormat = new Dictionary<string, List<string>>();
        
        foreach (var converter in converters)
        {
            var s = converter.Name.ToLower().Replace("converter", "");
            var sourceFormat = s.Split("to").First();
            var targetFormat = s.Split("to").Last();
            if (!convertersByTargetFormat.ContainsKey(targetFormat)) convertersByTargetFormat[targetFormat] = new List<string>();
            convertersByTargetFormat[targetFormat].Add(sourceFormat);
            if (!convertersBySourceFormat.ContainsKey(sourceFormat))
                convertersBySourceFormat[sourceFormat] = new List<string>();
            convertersBySourceFormat[sourceFormat].Add(targetFormat);
        }

        return new SupportedConversions
        {
            TargetFormatFrom = convertersByTargetFormat,
            SourceFormatTo = convertersBySourceFormat
        };
    }

    
}