using System.Reflection;
using ConvertX.To.API;
using ConvertX.To.API.Converters;
using ConvertX.To.API.Data;
using ConvertX.To.API.Extensions;
using ConvertX.To.API.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging();

builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true; // Using Filters/ValidationFilter.cs to do this
    });

builder.Services.AddControllers(options =>
    {
        options.Filters.RegisterFiltersFromAssembly(Assembly.GetExecutingAssembly());
    })
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });


builder.Services.AddHttpClient();

builder.AddAzureSettings();
builder.AddLocalFileServiceSettings();
builder.AddDatabaseSettings();


builder.Services.AddScoped<ILocalFileService, LocalFileService>();
builder.Services.AddScoped<IAzureFileService, SharePointFileService>();
builder.Services.AddScoped<IConverterFactory, ConverterFactory>();
builder.Services.AddScoped<IConversionEngine, ConversionEngine>();
builder.Services.AddScoped<IConversionService, ConversionService>();

builder.Services.AddScoped<IUriService>(provider =>
{
    var accessor = provider.GetRequiredService<IHttpContextAccessor>();
    var request = accessor.HttpContext.Request;
    var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
    return new UriService(absoluteUri);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>();

var app = builder.Build();

await app.RunPendingMigrationsAsync();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomExceptionHandlingMiddleware();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();