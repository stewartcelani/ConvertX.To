using ConvertX.To.Application.Interfaces;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace ConvertX.To.API.Controllers.V1;

/// <summary>
/// Used while debugging or working out LINQ EF-core queries
/// </summary>
[ApiController]
public class TestController : ControllerBase
{
    private readonly ApplicationDbContext _applicationDbContext;
    private readonly IConversionService _conversionService;

    public TestController(ApplicationDbContext applicationDbContext, IConversionService conversionService)
    {
        _applicationDbContext = applicationDbContext;
        _conversionService = conversionService;
    }

    [HttpGet("/test")]
    public async Task<IActionResult> Test()
    {
        /*var timeToLive = DateTimeOffset.Now.AddMinutes(-5);
        var conversions =
            _applicationDbContext.Conversions
                .Where(x => x.DateDeleted == null
                            && x.DateCreated < timeToLive)
                .Select(x => x.Id)
                .ToList();*/
        
        var nonExpiredConversionIds = _conversionService.GetAllIds().Select(x => x.ToString()).ToList();

        

        return Ok(nonExpiredConversionIds);
    }
}