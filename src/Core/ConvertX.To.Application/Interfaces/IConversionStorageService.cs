namespace ConvertX.To.Application.Interfaces;

public interface IConversionStorageService
{
    DirectoryInfo GetDirectory(string conversionId);
    DirectoryInfo GetRootDirectory();
    void DeleteConvertedFile(string conversionId);
    Stream GetConvertedFile(string conversionId);
    Task SaveConversionAsync(string conversionId, string convertedFileName, Stream stream);
}