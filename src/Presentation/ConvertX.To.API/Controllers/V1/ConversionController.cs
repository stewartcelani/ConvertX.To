using System.Reflection;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Domain.Settings;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IUriService _uriService;
    private readonly IConversionEngine _conversionEngine;
    private readonly IFileService _fileService;

    public ConversionController(IConversionService conversionService,
        IUriService uriService, IConversionEngine conversionEngine, IFileService fileService)
    {
        _conversionService = conversionService;
        _uriService = uriService;
        _conversionEngine = conversionEngine;
        _fileService = fileService;
    }

    [HttpPost(ApiRoutesV1.Convert.Post)]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Convert([FromRoute] string targetFormat, [FromForm] IFormFile file)
    {
        var requestDate = DateTimeOffset.Now;
        var sourceFormat = Path.GetExtension(file.FileName).ToLower().Replace(".", "");
        
        var convertedStream = await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, file.OpenReadStream());
        
        var conversion = new Conversion
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName),
            SourceFormat = sourceFormat,
            ConvertedFormat = targetFormat,
            RequestDate = requestDate,
            RequestCompleteDate = DateTimeOffset.Now
        };

        await _conversionService.CreateAsync(conversion);
        
        await _fileService.SaveFileAsync(Path.Combine(conversion.Id.ToString(), conversion.ConvertedFileName), convertedStream);
        await convertedStream.DisposeAsync();
        return Created(_uriService.GetFileUri(conversion.Id), new ConversionResponse { Id = conversion.Id.ToString() });
    }

    /// <summary>
    /// Returns list of supported conversions
    /// </summary>
    [HttpGet(ApiRoutesV1.Convert.Get)]
    public IActionResult GetSupportedConversions()
    {
        return Ok(_conversionEngine.GetSupportedConversions());
    }
}