using ConvertX.To.API.Converters;
using ConvertX.To.API.Data;
using ConvertX.To.API.Entities;
using ConvertX.To.API.Exceptions.Business;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.API.Services;

public class ConversionService : IConversionService
{
    private readonly ILogger<ConversionService> _logger;
    private readonly ILocalFileService _localFileService;
    private readonly IConversionEngine _conversionEngine;
    private readonly DataContext _dataContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ConversionService(IConversionEngine conversionEngine, ILogger<ConversionService> logger, ILocalFileService localFileService, DataContext dataContext, IHttpContextAccessor httpContextAccessor)
    {
        _conversionEngine = conversionEngine;
        _logger = logger;
        _localFileService = localFileService;
        _dataContext = dataContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Conversion> ConvertAsync(string targetFormat, IFormFile formFile)
    {
        _logger.LogDebug(nameof(ConversionService));
        var conversionTaskId = Guid.NewGuid();
        var sourceFile = await _localFileService.SaveFileAsync(conversionTaskId.ToString(), formFile);
        var conversionTask = new ConversionTask
        {
            Id = conversionTaskId,
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourceFile.FullName),
            SourceFileName = sourceFile.Name,
            SourceFilePath = sourceFile.FullName,
            DirectoryName = sourceFile.DirectoryName!,
            SourceFormat = sourceFile.Extension.ToLower().Replace(".", ""),
            TargetFormat = targetFormat.ToLower().Replace(".", ""),
            RequestDate = DateTimeOffset.Now
        };
        var conversionResult = await _conversionEngine.ConvertAsync(conversionTask);
        var conversion = conversionResult.Adapt<Conversion>();
        conversion.UserIpAddress = _httpContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? string
            .Empty;
        _dataContext.Conversions.Add(conversion);
        await _dataContext.SaveChangesAsync();
        return _dataContext.Conversions.Single(x => x.Id == conversion.Id);
    }

    public async Task<Conversion> GetByIdAsync(Guid conversionId)
    {
        return await _dataContext.Conversions.SingleAsync(x => x.Id == conversionId)
               ?? throw new ConversionNotFoundException($"Conversion with id {conversionId} not found in database.");
    }

    public async Task<Stream> GetConvertedFileAsStreamAsync(Guid conversionId)
    {
        var conversion = await GetByIdAsync(conversionId);
        conversion.Downloads++;
        _dataContext.Update(conversion);
        await _dataContext.SaveChangesAsync();
        return _localFileService.GetStream(conversionId.ToString(), conversion.ConvertedFileName);
    }
}