using ConvertX.To.Application;
using ConvertX.To.ConsoleUI.Core;
using ConvertX.To.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
 * This ConsoleUI.Core uses the conversion engine and related services directly and doesn't rely on the API.
 * It references:
 * - ConvertX.To.Application
 * - ConvertX.To.Infrastructure.Shared
 */
var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(((_, builder) => builder.AddJsonFile("appsettings.secret.json")))
    .ConfigureServices((context, services) =>
    {
        services.AddApplication();
        services.AddSharedInfrastructure(context.Configuration);
        services.AddHostedService<App>();
    })
    .Build();

await host.RunAsync();