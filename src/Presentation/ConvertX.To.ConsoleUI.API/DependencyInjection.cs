using ConvertX.To.Application.Helpers;
using ConvertX.To.ConsoleUI.API.ApiClient;
using ConvertX.To.ConsoleUI.API.ApiClient.DelegatingHandlers;
using ConvertX.To.ConsoleUI.API.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Refit;
using Serilog;

namespace ConvertX.To.ConsoleUI.API;

public static class DependencyInjection
{
    public static void AddConsoleUiApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog(Log.Logger, true);
        });

        var httpSettings = SettingsBinder.Bind<HttpSettings>(configuration);
        services.AddSingleton(httpSettings);
        
        AsyncRetryPolicy<HttpResponseMessage> retryPolicy = HttpPolicyExtensions
            .HandleTransientHttpError()
            .Or<TimeoutRejectedException>() // Thrown by Polly's TimeoutPolicy if the inner call gets timeout.
            .WaitAndRetryAsync(httpSettings.WaitAndRetryConfig.RetryAttempts, _ => TimeSpan.FromSeconds(httpSettings.WaitAndRetryConfig.RetrySeconds));
        
        AsyncTimeoutPolicy<HttpResponseMessage> timeoutPolicy = Policy
            .TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(httpSettings.WaitAndRetryConfig.TimeoutSeconds));

        /*services.AddTransient<ExceptionHandler>();*/
        services.AddTransient<UnsuccessfulStatusCodeHandler>();

        services.AddRefitClient<IApiClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(httpSettings.BaseUrl))
            /*.AddHttpMessageHandler<ExceptionHandler>()*/
            .AddHttpMessageHandler<UnsuccessfulStatusCodeHandler>()
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(timeoutPolicy);
        
        /*
            services.Decorate<IApiClient, ApiClient.ApiClient>();
        */

        //services.AddHttpClient();
    }
}