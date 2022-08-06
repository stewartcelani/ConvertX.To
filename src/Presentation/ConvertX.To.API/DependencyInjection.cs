using System.Reflection;
using ConvertX.To.API.Services;
using ConvertX.To.Application;
using ConvertX.To.Application.Helpers;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using ConvertX.To.Infrastructure.Persistence.Cron;
using ConvertX.To.Infrastructure.Persistence.Cron.Helpers;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.API;

public static class DependencyInjection
{
    public static void AddAspNetCoreServices(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true); // Using custom filter

        services.AddControllers(options =>
            options.Filters.RegisterFiltersFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddFluentValidation(options =>
            options.RegisterValidatorsFromAssemblyContaining<IApplicationMarker>());

        services.AddTransient<IUriService>(provider =>
        {
            var accessor = provider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor?.HttpContext?.Request;
            var absoluteUri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent());
            return new UriService(absoluteUri);
        });
    }

    private static void RegisterFiltersFromAssembly(this FilterCollection filterCollection, Assembly assembly)
    {
        var filters = ReflectionHelper.GetConcreteTypesInAssembly<IFilterMetadata>(assembly);
        filters.ForEach(filter => filterCollection.Add(filter));
    }

    public static async Task RunPendingMigrationsAsync(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if ((await dataContext.Database.GetPendingMigrationsAsync()).Any())
            await dataContext.Database.MigrateAsync();
    }

    public static void ScheduleRecurringJobs(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var recurringJobManager = serviceScope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<ConversionLifecycleManagerServiceScheduledTask>(
            nameof(ConversionLifecycleManagerServiceScheduledTask), x => x.RunAsync(),
            CronExpressionHelper.EveryMinutes(5));
    }
}