namespace ConvertX.To.Application.Interfaces;

public interface IConverter
{
    Task<Stream> ConvertAsync(string filePath);
}