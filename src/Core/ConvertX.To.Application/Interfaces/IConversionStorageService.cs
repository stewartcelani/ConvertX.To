namespace ConvertX.To.Application.Interfaces;

public interface IConversionStorageService
{
    DirectoryInfo GetDirectory(string conversionId);
    DirectoryInfo GetRootDirectory();
    void DeleteConvertedFile(Guid conversionId);
    Stream GetConvertedFile(Guid conversionId);
    Task SaveConversionAsync(Guid conversionId, string convertedFileName, Stream stream);
}