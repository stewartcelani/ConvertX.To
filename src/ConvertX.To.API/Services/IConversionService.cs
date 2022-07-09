using ConvertX.To.API.Entities;
using ConvertX.To.API.Interfaces;

namespace ConvertX.To.API.Services;

public interface IConversionService
{
    Task<FileInfo> ConvertAsync(string sourceFormat, string targetFormat, IFormFile formFile);
}