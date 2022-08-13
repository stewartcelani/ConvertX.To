using System;
using System.Diagnostics.CodeAnalysis;
using ConvertX.To.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Tests.Integration;

[ExcludeFromCodeCoverage]
public class ConvertXToApiFactory: WebApplicationFactory<IApiMarker>
{
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConvertXToApi_DatabaseSettings__ConnectionString", "Host=host.docker.internal;Username=postgres;Password=rydA191NNtUv;Database=ConvertXToTest;Port=5439;");
        Environment.SetEnvironmentVariable("ConvertXToApi_DatabaseSettings__EnableSensitiveDataLogging", "false");
        
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        /*builder.ConfigureTestServices(services =>
        {
        });*/
    }
    
}