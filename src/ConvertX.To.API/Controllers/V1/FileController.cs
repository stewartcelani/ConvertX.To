using ConvertX.To.API.Contracts.V1;
using Microsoft.AspNetCore.Mvc;

namespace ConvertX.To.API.Controllers.V1;

[ApiController]
public class FileController : ControllerBase
{
    private readonly ILogger<FileController> _logger;

    public FileController(ILogger<FileController> logger)
    {
        _logger = logger;
    }

    [HttpGet(ApiRoutes.Files.Get)]
    public IActionResult Get([FromRoute] string fileId)
    {
        _logger.LogDebug("ApiRoutes.Files.Get request");
        return Ok(fileId);
    }
    
}