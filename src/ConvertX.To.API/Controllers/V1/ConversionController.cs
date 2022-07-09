using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Contracts.V1.Responses;
using ConvertX.To.API.Services;
using Microsoft.AspNetCore.Mvc;
using MimeTypes.Core;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class ConversionController : ControllerBase
{
    private readonly ILogger<ConversionController> _logger;
    private readonly IConversionService _conversionService;
    private readonly IFileService _fileService;
    private readonly IUriService _uriService;
    
    public ConversionController(ILogger<ConversionController> logger, IConversionService conversionService, IUriService uriService, IFileService fileService)
    {
        _logger = logger;
        _conversionService = conversionService;
        _uriService = uriService;
        _fileService = fileService;
    }

    [HttpPost(ApiRoutes.Convert.Post)]
    public async Task<IActionResult> Convert([FromRoute]string from, [FromRoute]string to, IFormFile file)
    {
        _logger.LogDebug($"{nameof(ConversionController)}.{nameof(Convert)}");
        var conversionResult = await _conversionService.ConvertAsync(from, to, file);
        var stream = _fileService.GetStream(conversionResult.FullName);
        return new FileStreamResult(stream, MimeTypeMap.GetMimeType(conversionResult.Extension));

        // TODO: Return a 201 with just the Guid of converted file, store file conversion data in DB, move the download
        // to its own controller
        //var locationUri = _uriService.GetFileUri(conversionResult.FileId);
        /*return Created(locationUri, new ConversionResponse
        {
            FileId = conversionResult.FileId.ToString()
        });*/



    }
    
}