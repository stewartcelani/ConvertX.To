using ConvertX.To.API.Contracts.V1;
using ConvertX.To.API.Services;
using Microsoft.AspNetCore.Mvc;
using MimeTypes.Core;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;
    private readonly IConversionService _conversionService;

    public FileController(ILogger<FileController> logger, IConversionService conversionService)
    {
        _logger = logger;
        _conversionService = conversionService;
    }

    [HttpGet(ApiRoutes.Files.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid conversionId)
    {
        _logger.LogDebug($"{nameof(FileController)}.{nameof(Get)}");
        var stream = (FileStream)await _conversionService.GetConvertedFileAsStreamAsync(conversionId);
        return new FileStreamResult(stream, MimeTypeMap.GetMimeType(Path.GetExtension(stream.Name)));
    }
    
}