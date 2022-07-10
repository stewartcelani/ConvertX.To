using System.Net.Mime;
using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.API.Entities;
using ConvertX.To.API.Services;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.StaticFiles;
using MimeTypes.Core;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly ILogger<ConversionController> _logger;
    private readonly IConversionService _conversionService;
    private readonly IUriService _uriService;

    public ConversionController(ILogger<ConversionController> logger, IConversionService conversionService,
        IUriService uriService)
    {
        _logger = logger;
        _conversionService = conversionService;
        _uriService = uriService;
    }

    [HttpPost(ApiRoutes.Convert.Post)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Convert([FromRoute] string targetFormat, [FromForm] IFormFile file)
    {
        _logger.LogDebug($"{nameof(ConversionController)}.{nameof(Convert)}");
        var conversion = await _conversionService.ConvertAsync(targetFormat, file);
        return Created(_uriService.GetFileUri(conversion.Id), new ConversionResponse { Id = conversion.Id.ToString() });
    }
}