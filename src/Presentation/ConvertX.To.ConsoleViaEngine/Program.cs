using ConvertX.To.Application;
using ConvertX.To.ConsoleViaEngine;
using ConvertX.To.Infrastructure.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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