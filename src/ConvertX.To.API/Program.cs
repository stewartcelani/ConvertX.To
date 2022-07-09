using System.Reflection;
using ConvertX.To.API.Converters;
using ConvertX.To.API.Logging;
using ConvertX.To.API.Filters;
using ConvertX.To.API.Middleware;
using ConvertX.To.API.Services;
using ConvertX.To.API.Settings;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging();

// Add services to the container.
builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.SuppressModelStateInvalidFilter = true; // Using ValidationFilter below to do this
    });

builder.Services.AddControllers(options =>
    {
        options.Filters.RegisterFiltersFromAssembly(Assembly.GetExecutingAssembly());
    })
    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

var azureSettings = new AzureSettings();
builder.Configuration.AddJsonFile("appsettings.secret.json"); // Ignored by .gitignore
builder.Configuration.GetSection(nameof(AzureSettings)).Bind(azureSettings);
builder.Services.AddSingleton(azureSettings);

builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IConverterFactory, ConverterFactory>();
builder.Services.AddScoped<IConversionEngine, ConversionEngine>();
builder.Services.AddScoped<IConversionService, ConversionService>();
//builder.Services.AddScoped<IConverter, DocxToPdfConverter>();

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

var app = builder.Build();

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