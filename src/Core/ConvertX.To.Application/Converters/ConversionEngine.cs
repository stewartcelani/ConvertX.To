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

    // TODO: 1) Implement remaining MsGraph to Jpg converters
    // TODO: 2) Use something like http://www.pdfsharp.net/ to convert from Jpg to Pdf
    // TODO: 3) Can then transparently convert to jpg using MsGraph then to Pdf from Jpg using PdfSharp and support many more formats
    public async Task<Stream> ConvertAsync(string sourceFormat, string targetFormat, Stream source)
    {
        _logger.LogInformation(
            $"Processing new request to convert {sourceFormat} to {targetFormat}");

        var converter = _converterFactory.Create(sourceFormat, targetFormat);

        return await converter.ConvertAsync(source);
    }
    
}