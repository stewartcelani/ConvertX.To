using System.Reflection;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Domain.Settings;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IUriService _uriService;
    private readonly IConversionEngine _conversionEngine;
    private readonly IFileService _fileService;
    private readonly ILogger<ConversionController> _logger;

    public ConversionController(IConversionService conversionService,
        IUriService uriService, IConversionEngine conversionEngine, IFileService fileService, ILogger<ConversionController> logger)
    {
        _conversionService = conversionService;
        _uriService = uriService;
        _conversionEngine = conversionEngine;
        _fileService = fileService;
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
        
        var conversion = new Conversion
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName),
            SourceFormat = sourceFormat,
            TargetFormat = targetFormat,
            ConvertedFileExtension = convertedFileExtension,
            RequestDate = requestDate,
            RequestCompleteDate = DateTimeOffset.Now
        };

        await _conversionService.CreateAsync(conversion);
        
        await _fileService.SaveFileAsync(Path.Combine(conversion.Id.ToString(), conversion.ConvertedFileName), convertedStream);
        await convertedStream.DisposeAsync();
        
        return Created(_uriService.GetFileUri(conversion.Id), conversion.Adapt<ConversionResponse>());
    }
    
    /// <summary>
    /// Returns list of supported conversions
    /// </summary>
    [HttpGet(ApiRoutesV1.Convert.Get)]
    public IActionResult GetSupportedConversions()
    {
        return Ok(_conversionEngine.GetSupportedConversions().Adapt<SupportedConversionsResponse>());
    }
}