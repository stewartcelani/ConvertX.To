using ConvertX.To.Application;
using ConvertX.To.ConsoleUI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(((_, builder) => builder.AddJsonFile("appsettings.secret.json")))
    .ConfigureServices((context, services) =>
    {
        services.AddApplication();
        services.AddConversionEngine(context.Configuration);
        services.AddScoped<App>();
    })
    .Build();

var app = host.Services.GetRequiredService<App>();
await app.RunAsync(args);