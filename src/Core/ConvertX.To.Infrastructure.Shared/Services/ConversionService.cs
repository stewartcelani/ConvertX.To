using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Exceptions.Business;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionService : IConversionService
{
    private readonly IFileService _fileService;
    private readonly IConversionEngine _conversionEngine;
    private readonly IIpAddressService _ipAddressService;
    private readonly ApplicationDbContext _applicationDbContext;
    
    public ConversionService(IConversionEngine conversionEngine, IFileService fileService, ApplicationDbContext applicationDbContext, IIpAddressService ipAddressService)
    {
        _conversionEngine = conversionEngine;
        _fileService = fileService;
        _applicationDbContext = applicationDbContext;
        _ipAddressService = ipAddressService;
    }

    public async Task<Conversion> ConvertAsync(string targetFormat, string fileName, Stream stream)
    {
        var conversionTaskId = Guid.NewGuid();
        var sourceFile = await _fileService.SaveFileAsync(conversionTaskId.ToString(), fileName, stream);
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
        conversion.ConvertedFormat = conversionResult.TargetFormat;
        conversion.UserIpAddress = _ipAddressService.GetUserIpAddress();

        _applicationDbContext.Conversions.Add(conversion);
        await _applicationDbContext.SaveChangesAsync();
        return conversion;
    }
    
    public async Task<Stream> DownloadFileAsync(Guid conversionId)
    {
        var conversion = await GetByIdAsync(conversionId);
        conversion.Downloads++;
        _applicationDbContext.Update(conversion);
        await _applicationDbContext.SaveChangesAsync();
        return _fileService.GetStream(conversionId.ToString(), conversion.ConvertedFileName);
    }
    
    public async Task<Conversion> GetByIdAsync(Guid conversionId)
    {
        try
        {
            return await _applicationDbContext.Conversions.FirstAsync(x => x.Id == conversionId);
        }
        catch (InvalidOperationException)
        {
            throw new ConversionNotFoundException($"Conversion with id {conversionId} not found in database.");
        }
    }
}