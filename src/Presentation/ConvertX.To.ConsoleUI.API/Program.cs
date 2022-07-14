using ConvertX.To.ConsoleUI.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/*
 * This ConsoleUI.API uses the API to handle all conversions and only references:
 * - ConvertX.To.Application
 * - ConvertX.To.Domain
 * - ConvertX.To.API.Contracts
 */
var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddConsoleUiApiServices(context.Configuration);
        services.AddHostedService<App>();
    })
    .Build();

await host.RunAsync();
