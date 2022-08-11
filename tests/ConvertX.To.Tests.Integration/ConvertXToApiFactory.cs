using System.Diagnostics.CodeAnalysis;
using ConvertX.To.API;
using ConvertX.To.Application.Domain.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace ConvertX.To.Tests.Integration;

[ExcludeFromCodeCoverage]
public class ConvertXToApiFactory: WebApplicationFactory<IApiMarker>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DatabaseSettings>();
            var databaseSettings = new DatabaseSettings
            {
                ConnectionString =
                    "Host=host.docker.internal;Username=postgres;Password=rydA191NNtUv;Database=ConvertXToTest;Port=5439;",
                EnableSensitiveDataLogging = false,
            };
            services.AddSingleton(databaseSettings);
        });
    }
    
}