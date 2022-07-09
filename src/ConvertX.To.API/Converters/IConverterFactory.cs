namespace ConvertX.To.API.Converters;

public interface IConverterFactory
{
    IConverter Create(string from, string to);
}