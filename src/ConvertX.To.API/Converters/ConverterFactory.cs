using ConvertX.To.API.Exceptions;
using ConvertX.To.API.Exceptions.Business;
using ConvertX.To.API.Extensions;
using ConvertX.To.API.Services;

namespace ConvertX.To.API.Converters;

public class ConverterFactory : IConverterFactory
{
    private readonly ILogger<ConverterFactory> _logger;
    private readonly IAzureFileService _azureFileService;

    public ConverterFactory(ILogger<ConverterFactory> logger, IAzureFileService azureFileService)
    {
        _logger = logger;
        _azureFileService = azureFileService;
    }

    public IConverter Create(string from, string to)
    {
        try
        {
            var typeFullName = typeof(ConverterFactory).Namespace + "." + from.Proper() + "To" + to.Proper() + "Converter";
            var type = Type.GetType(typeFullName);
            var constructorParams = new object[] { _logger };
            if (type?.BaseType == typeof(AzureConverter))
            {
                constructorParams = new object[] { _azureFileService, _logger };
            }
            
            return (IConverter)Activator.CreateInstance(Type.GetType(typeFullName), constructorParams);
        }
        catch
        {
            throw new UnsupportedConversionException($"Converting from {from.Proper()} to {to.Proper()} is not supported.");
        }
    }
}

