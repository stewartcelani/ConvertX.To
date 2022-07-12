using System.Reflection;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Exceptions.Business;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Entities;
using ConvertX.To.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class ConversionService : IConversionService
{
    private readonly IStorageService _storageService;
    private readonly IConversionEngine _conversionEngine;
    private readonly IIpAddressService _ipAddressService;
    private readonly ApplicationDbContext _applicationDbContext;

    public ConversionService(IConversionEngine conversionEngine, IStorageService storageService,
        ApplicationDbContext applicationDbContext, IIpAddressService ipAddressService)
    {
        _conversionEngine = conversionEngine;
        _storageService = storageService;
        _applicationDbContext = applicationDbContext;
        _ipAddressService = ipAddressService;
    }

    public async Task<Conversion> ConvertAsync(string targetFormat, string fileName, Stream stream)
    {
        var requestDate = DateTimeOffset.Now;
        var sourceFormat = Path.GetExtension(fileName).ToLower().Replace(".", "");

        var convertedStream = await _conversionEngine.ConvertAsync(sourceFormat, targetFormat, stream);

        var conversion = new Conversion
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName),
            SourceFormat = sourceFormat,
            ConvertedFormat = targetFormat,
            RequestDate = requestDate,
            RequestCompleteDate = DateTimeOffset.Now,
            UserIpAddress = _ipAddressService.GetUserIpAddress()
        };

        _applicationDbContext.Conversions.Add(conversion);
        await _applicationDbContext.SaveChangesAsync();

        try
        {
            await _storageService.SaveFileAsync(conversion.Id.ToString(), conversion.ConvertedFileName,
                convertedStream);
        }
        catch (Exception ex)
        {
            // TODO: Implement deleting the added conversion above since the file never got saved, then throw
            // the error again to be handled by middleware
            //await _storageService.DeleteFileAsync(conversion.Id.ToString(), conversion.ConvertedFileName);
            throw;
        }

        return conversion;
    }

    public async Task<Stream> DownloadFileAsync(Guid conversionId)
    {
        var conversion = await GetByIdAsync(conversionId);
        conversion.Downloads++;
        _applicationDbContext.Update(conversion);
        await _applicationDbContext.SaveChangesAsync();
        return _storageService.GetFileAsync(conversionId.ToString(), conversion.ConvertedFileName);
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
    
    public SupportedConversions GetSupportedConversions()
    {
        var assembly = Assembly.GetAssembly(typeof(IConversionEngine))!;
        
        var converters = assembly.ExportedTypes
            .Where(x => typeof(IConverter).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .ToList();

        var convertersByTargetFormat = new Dictionary<string, List<string>>();
        var convertersBySourceFormat = new Dictionary<string, List<string>>();
        foreach (var converter in converters)
        {
            var s = converter.Name.ToLower().Replace("converter", "");
            var sourceFormat = s.Split("to").First();
            var targetFormat = s.Split("to").Last();
            if (!convertersByTargetFormat.ContainsKey(targetFormat)) convertersByTargetFormat[targetFormat] = new List<string>();
            convertersByTargetFormat[targetFormat].Add(sourceFormat);
            if (!convertersBySourceFormat.ContainsKey(sourceFormat))
                convertersBySourceFormat[sourceFormat] = new List<string>();
            convertersBySourceFormat[sourceFormat].Add(targetFormat);
        }

        return new SupportedConversions
        {
            TargetFormatFrom = convertersByTargetFormat,
            SourceFormatTo = convertersBySourceFormat
        };
    }
}