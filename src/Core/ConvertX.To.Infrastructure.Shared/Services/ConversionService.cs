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
        var sourceFormat = Path.GetExtension(fileName).ToLower().Replace(".","");
        var requestDate = DateTimeOffset.Now;
        
        var convertedStream = await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, stream);
        
        var conversion = new Conversion
        {
            Id = Guid.NewGuid(),
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName),
            SourceFormat = sourceFormat,
            ConvertedFormat = targetFormat,
            RequestDate = requestDate,
            RequestCompleteDate = DateTimeOffset.Now,
            UserIpAddress = _ipAddressService.GetUserIpAddress()
        };
        
        await _fileService.SaveFileAsync(conversion.Id.ToString(), conversion.ConvertedFileName, convertedStream);
        
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