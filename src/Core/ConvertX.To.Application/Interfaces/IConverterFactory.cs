namespace ConvertX.To.Application.Interfaces;

public interface IConverterFactory
{
    IConverter Create(string sourceFormat, string targetFormat);
}