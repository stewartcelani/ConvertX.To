using ConvertX.To.API.Entities;
using ConvertX.To.API.Interfaces;

namespace ConvertX.To.API.Converters;

public interface IConversionEngine
{
    Task<FileInfo> ConvertAsync(string sourceFormat, string targetFormat, FileInfo sourceFile);
}