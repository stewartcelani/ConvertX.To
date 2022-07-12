using ConvertX.To.API.Contracts.V1;
using ConvertX.To.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MimeTypes.Core;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class FileController : ControllerBase
{
    private readonly IConversionService _conversionService;

    public FileController(IConversionService conversionService)
    {
        _conversionService = conversionService;
    }

    [HttpGet(ApiRoutesV1.Files.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid conversionId)
    {
        var stream = (FileStream)await _conversionService.DownloadFileAsync(conversionId);
        return new FileStreamResult(stream, MimeTypeMap.GetMimeType(Path.GetExtension(stream.Name)));
    }
    
}