using System.Reflection;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Options;

namespace ConvertX.To.Application.Converters;

public class ConversionEngine : IConversionEngine
{
    private readonly IConverterFactory _converterFactory;
    private readonly ILoggerAdapter<ConversionEngine> _logger;

    public ConversionEngine(ILoggerAdapter<ConversionEngine> logger,
        IConverterFactory converterFactory)
    {
        _logger = logger;
        _converterFactory = converterFactory;
    }

    // TODO: MsGraph has a 250 page limit when converting to PDF so implement a ToPdfConverterBase and check if pages > 250 and if so, split it
    // TODO: Look into QuestPdf: https://www.questpdf.com/documentation/api-reference.html#static-images
    // Could try use it to convert Jpg to Pdf and to also write .txt, .log, .json (text formats) to PDF which can then be converted into JPG

    public async Task<(string, Stream)> ConvertAsync(string sourceFormat, string targetFormat, Stream source,
        ConversionOptions conversionOptions)
    {
        _logger.LogDebug(
            "Processing new request to convert {sourceFormat} to {targetFormat}", sourceFormat, targetFormat);

        var converter = _converterFactory.Create(sourceFormat, targetFormat);

        return await converter.ConvertAsync(source, conversionOptions);
    }

    public static SupportedConversions GetSupportedConversions()
    {
        var convertersByTargetFormat = new Dictionary<string, List<string>>();
        var convertersBySourceFormat = new Dictionary<string, List<string>>();

        foreach (var converter in
                 ReflectionHelper.GetConcreteTypesInAssembly<IConverter>(Assembly.GetExecutingAssembly()))
            MapSupportedConversionsForConverter(converter, convertersByTargetFormat, convertersBySourceFormat);

        return new SupportedConversions
        {
            TargetFormatFrom = convertersByTargetFormat,
            SourceFormatTo = convertersBySourceFormat
        };
    }

    private static void MapSupportedConversionsForConverter(Type converter,
        IDictionary<string, List<string>> convertersByTargetFormat,
        IDictionary<string, List<string>> convertersBySourceFormat)
    {
        var (sourceFormat, targetFormat) = ParseFormats(converter.Name);

        if (!convertersByTargetFormat.ContainsKey(targetFormat))
            convertersByTargetFormat[targetFormat] = new List<string>();
        convertersByTargetFormat[targetFormat].Add(sourceFormat);

        if (!convertersBySourceFormat.ContainsKey(sourceFormat))
            convertersBySourceFormat[sourceFormat] = new List<string>();
        convertersBySourceFormat[sourceFormat].Add(targetFormat);
    }


    private static (string, string) ParseFormats(string converterName)
    {
        var s = converterName.ToLower().Replace("converter", "");
        return (s.Split("to").First(), s.Split("to").Last());
    }
}