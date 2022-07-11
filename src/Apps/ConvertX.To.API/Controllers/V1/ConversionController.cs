using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IUriService _uriService;

    public ConversionController(IConversionService conversionService,
        IUriService uriService)
    {
        _conversionService = conversionService;
        _uriService = uriService;
    }

    [HttpPost(ApiRoutesV1.Convert.Post)]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Convert([FromRoute] string targetFormat, [FromForm] IFormFile file)
    {
        var conversion = await _conversionService.ConvertAsync(targetFormat, file.FileName, file.OpenReadStream());
        return Created(_uriService.GetFileUri(conversion.Id), new ConversionResponse { Id = conversion.Id.ToString() });
    }
}