using System.Text.Json;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Mappers;
using ConvertX.To.API.Contracts.V1.Queries;
using ConvertX.To.API.Services;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Extensions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Application.Validators.Helpers;
using ConvertX.To.Domain;
using ConvertX.To.Domain.Options;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IConversionEngine _conversionEngine;
    private readonly IConversionStorageService _conversionStorageService;
    private readonly ILoggerAdapter<ConversionController> _logger;
    private readonly ConversionLifecycleManagerSettings _conversionLifecycleManagerSettings;
    private readonly IUriService _uriService;

    public ConversionController(IConversionService conversionService, IConversionEngine conversionEngine,
        IConversionStorageService conversionStorageService, ILoggerAdapter<ConversionController> logger,
        ConversionLifecycleManagerSettings conversionLifecycleManagerSettings, IUriService uriService)
    {
        _conversionService = conversionService ?? throw new NullReferenceException(nameof(conversionService));
        _conversionEngine = conversionEngine ?? throw new NullReferenceException(nameof(conversionEngine));
        _conversionStorageService = conversionStorageService ??
                                    throw new NullReferenceException(nameof(conversionStorageService));
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _conversionLifecycleManagerSettings = conversionLifecycleManagerSettings ??
                                              throw new NullReferenceException(
                                                  nameof(conversionLifecycleManagerSettings));
        _uriService = uriService ?? throw new NullReferenceException(nameof(uriService));
    }
    
    /// <summary>
    ///     Returns list of supported conversions
    /// </summary>
    [HttpGet(ApiRoutesV1.Convert.Get.Url)]
    public IActionResult GetSupportedConversions()
    {
        return Ok(ConversionEngine.GetSupportedConversions().ToSupportedConversionsResponse());
    }

    [HttpPost(ApiRoutesV1.Convert.Post.Url)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ConvertAsync([FromRoute] string targetFormat, [FromForm] IFormFile file, [FromQuery] ConversionOptionsQuery conversionOptionsQuery)
    {
        if (file.Length == 0) throw new InvalidFileLengthException();

        var requestDate = DateTimeOffset.Now;
        
        var sourceFormat = Path.GetExtension(file.FileName).ToLower().Replace(".", "");

        _logger.LogInformation("Conversion request: {sourceFormat} to {targetFormat}", sourceFormat, targetFormat);
        
        var conversionOptions = conversionOptionsQuery.ToConversionOptions();
        
        var (convertedFileExtension, convertedStream) =
            await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, file.OpenReadStream(), conversionOptions);

        var now = DateTimeOffset.Now;
        var conversion = new Conversion
        {
            SourceFormat = sourceFormat,
            TargetFormat = targetFormat,
            ConvertedFormat = convertedFileExtension,
            SourceMegabytes = file.Length.ToMegabytes(),
            ConvertedMegabytes = convertedStream.Length.ToMegabytes(),
            DateRequestReceived = requestDate,
            DateRequestCompleted = now
        };

        var created = await _conversionService.CreateAsync(conversion);

        if (!created)
        {
            var message = $"There was an unexpected error creating conversion: {JsonSerializer.Serialize(conversion)}";
            throw new ApiException(message, ValidationFailureHelper.Generate(nameof(Conversion), message));
        }

        var convertedFileName = _conversionService.GetConvertedFileName(Path.GetFileNameWithoutExtension(file.FileName),
            conversion.TargetFormat, conversion.ConvertedFormat);

        await _conversionStorageService.SaveConversionAsync(conversion.Id, convertedFileName,
            convertedStream);

        await convertedStream.DisposeAsync();

        var conversionResponse =
            conversion.ToConversionResponse(_conversionLifecycleManagerSettings.TimeToLiveInMinutes);

        return Created(_uriService.GetFileUri(conversion.Id), conversionResponse);
    }


   
}