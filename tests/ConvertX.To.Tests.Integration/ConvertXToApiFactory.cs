using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using ConvertX.To.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Xunit;

namespace ConvertX.To.Tests.Integration;

[ExcludeFromCodeCoverage]
public class ConvertXToApiFactory : WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable("ConvertXToApi_DatabaseSettings__ConnectionString",
            "Host=host.docker.internal;Username=postgres;Password=rydA191NNtUv;Database=ConvertXToTest;Port=5439;");
        Environment.SetEnvironmentVariable("ConvertXToApi_DatabaseSettings__EnableSensitiveDataLogging", "false");
        Environment.SetEnvironmentVariable("ConvertXToApi_MsGraphSettings__AuthenticationEndpoint",
            "http://localhost:51923");
        Environment.SetEnvironmentVariable("ConvertXToApi_MsGraphSettings__GraphEndpoint", "http://localhost:51923");

        builder.ConfigureLogging(logging => { logging.ClearProviders(); });

        /*builder.ConfigureTestServices(services =>
        {
        });*/
    }

}