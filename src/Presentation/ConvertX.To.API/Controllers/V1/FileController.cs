using ConvertX.To.API.Contracts.V1;
using ConvertX.To.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using MimeTypes.Core;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class FileController : ControllerBase
{
    private readonly IConversionService _conversionService;
    private readonly IFileService _fileService;

    public FileController(IConversionService conversionService, IFileService fileService)
    {
        _conversionService = conversionService;
        _fileService = fileService;
    }

    [HttpGet(ApiRoutesV1.Files.Get)]
    public async Task<IActionResult> Get([FromRoute] Guid conversionId)
    {
        var conversion = await _conversionService.GetByIdAsync(conversionId);
        if (conversion.DateDeleted is not null) return StatusCode(410, null);
        
        var stream = (FileStream)_fileService.GetFile(Path.Combine(conversion.Id.ToString(), conversion.ConvertedFileName));
        
        conversion.Downloads++;
        await _conversionService.UpdateAsync(conversion);
        
        return new FileStreamResult(stream, MimeTypeMap.GetMimeType(Path.GetExtension(stream.Name)));
    }
    
    [HttpDelete(ApiRoutesV1.Files.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid conversionId)
    {
        var conversion = await _conversionService.GetByIdAsync(conversionId);
        if (conversion.DateDeleted is not null) return Ok();

        _fileService.DeleteDirectory(conversion.Id.ToString());
        
        conversion.DateDeleted = DateTimeOffset.Now;
        await _conversionService.UpdateAsync(conversion);

        return Ok();
    }
    
}