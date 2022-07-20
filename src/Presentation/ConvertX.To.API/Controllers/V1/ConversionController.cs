using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Mappers;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Extensions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IUriService _uriService;
    private readonly IConversionEngine _conversionEngine;
    private readonly IConversionStorageService _conversionStorageService;
    private readonly ILogger<ConversionController> _logger;

    public ConversionController(IConversionService conversionService,
        IUriService uriService, IConversionEngine conversionEngine, IConversionStorageService conversionStorageService, ILogger<ConversionController> logger)
    {
        _conversionService = conversionService;
        _uriService = uriService;
        _conversionEngine = conversionEngine;
        _conversionStorageService = conversionStorageService;
        _logger = logger;
    }

    [HttpPost(ApiRoutesV1.Convert.Post)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Convert([FromRoute] string targetFormat, [FromForm] IFormFile file)
    {
        if (file.Length == 0) throw new InvalidFileLengthException();
        
        _logger.LogInformation("Conversion request: {fileName} to {targetFormat}", file.FileName, targetFormat);
        
        var requestDate = DateTimeOffset.Now;
        var sourceFormat = Path.GetExtension(file.FileName).ToLower().Replace(".", "");

        var conversionOptions = new ConversionOptions();
        var (convertedFileExtension, convertedStream) = await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, file.OpenReadStream(), conversionOptions);

        var now = DateTimeOffset.Now;
        var conversion = new Conversion
        {
            SourceFormat = sourceFormat,
            TargetFormat = targetFormat,
            ConvertedFormat = convertedFileExtension,
            SourceMegabytes = file.Length.ToMegabytes(),
            ConvertedMegabytes = convertedStream.Length.ToMegabytes(),
            RequestDate = requestDate,
            RequestCompleteDate = now,
            RequestSeconds = (decimal)(now - requestDate).TotalSeconds
        };

        await _conversionService.CreateAsync(conversion);

        var convertedFileName = _conversionService.GetConvertedFileName(Path.GetFileNameWithoutExtension(file.FileName),
            conversion.TargetFormat, conversion.ConvertedFormat);

        await _conversionStorageService.SaveConversionAsync(conversion.Id.ToString(), convertedFileName,
            convertedStream);
        
        await convertedStream.DisposeAsync();
        
        return Created(_uriService.GetFileUri(conversion.Id), conversion.ToConversionResponse());
    }
    
    
    
    /// <summary>
    /// Returns list of supported conversions
    /// </summary>
    [HttpGet(ApiRoutesV1.Convert.Get)]
    public IActionResult GetSupportedConversions()
    {
        return Ok(_conversionEngine.GetSupportedConversions().ToSupportedConversionsResponse());
    }
}