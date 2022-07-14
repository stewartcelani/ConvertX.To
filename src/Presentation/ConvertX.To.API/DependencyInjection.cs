using System.Reflection;
using ConvertX.To.API.Services;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Helpers;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.API;

public static class DependencyInjection
{
    public static void AddAspNetCoreServices(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true); // Using custom filter

        services.AddControllers(options =>
            options.Filters.RegisterFiltersFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddFluentValidation(options =>
            options.RegisterValidatorsFromAssemblyContaining<ConvertXToExceptionBase>());
        
        services.AddTransient<IUriService>(provider =>
        {
            var accessor = provider.GetRequiredService<IHttpContextAccessor>();
            var request = accessor?.HttpContext?.Request;
            var absoluteUri = string.Concat(request?.Scheme, "://", request?.Host.ToUriComponent(), "/");
            return new UriService(absoluteUri);
        });

        services.AddTransient<IIpAddressService, IpAddressService>();
    }
    
    private static void RegisterFiltersFromAssembly(this FilterCollection filterCollection, Assembly assembly)
    {
        var filters = Reflection.GetConcreteTypesInAssembly<IFilterMetadata>(assembly);
        filters.ForEach(filter => filterCollection.Add(filter));
    }
 
    public static async Task RunPendingMigrationsAsync(this WebApplication app)
    {
        using var serviceScope = app.Services.CreateScope();
        var dataContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (dataContext.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory") // IntegrationTests
            if ((await dataContext.Database.GetPendingMigrationsAsync()).Any())
                await dataContext.Database.MigrateAsync();
    }
}