using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.API.Services;

public class IpAddressService : IIpAddressService
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public IpAddressService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserIpAddress()
    {
        if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            return _httpContextAccessor.HttpContext.Request.Headers["X-Forwarded-For"];
       
        return _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}