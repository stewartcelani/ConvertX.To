using ConvertX.To.API.Interfaces;

namespace ConvertX.To.API.Converters;

public interface IConverter
{
    Task<FileInfo> ConvertAsync(FileInfo file);
}