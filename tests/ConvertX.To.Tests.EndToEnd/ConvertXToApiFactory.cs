using ConvertX.To.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Tests.EndToEnd;


public class ConvertXToApiFactory : WebApplicationFactory<IApiMarker>
{
    public const int Port = 5432;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("https_port", Port.ToString());

        Environment.SetEnvironmentVariable("ConvertXToApi_DatabaseSettings__ConnectionString",
            "Host=host.docker.internal;Username=postgres;Password=wydB191DRtSv;Database=ConvertXToEndToEndTest;Port=5539;");
        Environment.SetEnvironmentVariable("ConvertXToApi_DatabaseSettings__EnableSensitiveDataLogging", "false");

        /*
        builder.ConfigureLogging(logging => { logging.ClearProviders(); });
        */

    }

}