using ConvertX.To.API.Contracts.V1;
using ConvertX.To.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MimeTypes.Core;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class FileController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IConversionStorageService _conversionStorageService;

    public FileController(IConversionService conversionService, IConversionStorageService conversionStorageService)
    {
        _conversionService = conversionService;
        _conversionStorageService = conversionStorageService;
    }

    [HttpGet(ApiRoutesV1.Files.Get.Url)]
    public async Task<IActionResult> GetFile([FromRoute] Guid conversionId)
    {
        if (!await _conversionService.ExistsAsync(conversionId)) return NotFound();

        var stream = (FileStream)_conversionStorageService.GetConvertedFile(conversionId);

        await _conversionService.IncrementDownloadCounter(conversionId);

        return new FileStreamResult(stream, MimeTypeMap.GetMimeType(Path.GetExtension(stream.Name)));
    }

    [HttpDelete(ApiRoutesV1.Files.Delete.Url)]
    public async Task<IActionResult> DeleteFile([FromRoute] Guid conversionId)
    {
        if (!await _conversionService.ExistsAsync(conversionId)) return NotFound();

        _conversionStorageService.DeleteConvertedFile(conversionId);

        await _conversionService.DeleteAsync(conversionId);

        return Ok();
    }
}