using ConvertX.To.API;
using ConvertX.To.API.Middleware;
using ConvertX.To.Application;
using ConvertX.To.Infrastructure.Persistence;
using ConvertX.To.Infrastructure.Shared;
using Hangfire;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.secret.json"); // Microsoft Graph Settings, ignored by .gitignore
builder.Configuration.AddEnvironmentVariables("ConvertXToApi_");

builder.Services.AddApplication();
builder.Services.AddPersistenceInfrastructure(builder.Configuration);
builder.Services.AddSharedInfrastructure(builder.Configuration);
builder.Services.AddAspNetCoreServices();

if (builder.Environment.IsDevelopment())
{
    builder.Services
        .AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddSwaggerGen();    
}


var app = builder.Build();

await app.RunPendingMigrationsAsync();

app.ScheduleRecurringJobs();
app.UseHangfireDashboard();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<ApiExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHangfireDashboard();
});

app.Run();