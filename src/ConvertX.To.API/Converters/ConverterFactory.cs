using System.Runtime.CompilerServices;
using ConvertX.To.API.Exceptions;
using ConvertX.To.API.Settings;

namespace ConvertX.To.API.Converters;

public class ConverterFactory : IConverterFactory
{
    private readonly ILogger<ConverterFactory> _logger;
    private readonly AzureSettings _azureSettings;

    public ConverterFactory(ILogger<ConverterFactory> logger, AzureSettings azureSettings)
    {
        _logger = logger;
        _azureSettings = azureSettings;
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
                constructorParams = new object[] { _azureSettings, _logger };
            } 
            return (IConverter)Activator.CreateInstance(Type.GetType(typeFullName), constructorParams);
        }
        catch
        {
            throw new UnsupportedConversionException($"Converting from {from.Proper()} to {to.Proper()} is not supported.");
        }
    }
}

public static class ProperExtension
{
    public static string Proper(this string s)
    {
        return s.First().ToString().ToUpper() + s.ToLower()[1..];
    }
}