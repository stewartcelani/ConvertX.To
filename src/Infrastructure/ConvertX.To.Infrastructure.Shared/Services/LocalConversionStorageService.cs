using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class LocalConversionStorageService : IConversionStorageService
{
    private readonly string _rootDirectory;

    public LocalConversionStorageService(ConversionStorageSettings conversionStorageSettings)
    {
        _rootDirectory = conversionStorageSettings.RootDirectory ??
                         throw new NullReferenceException(nameof(conversionStorageSettings.RootDirectory));
    }

    public async Task SaveConversionAsync(string conversionId, string convertedFileName, Stream stream)
    {
        EnsureDirectory(conversionId);
        await using var fileStream = new FileStream(Path.Combine(_rootDirectory, conversionId, convertedFileName), FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
        await fileStream.DisposeAsync();
    }

    public DirectoryInfo GetDirectory(string conversionId) => new (Path.Combine(_rootDirectory, conversionId));

    public DirectoryInfo GetRootDirectory() => GetDirectory(_rootDirectory);

    public void DeleteConvertedFile(string conversionId) => Directory.Delete(Path.Combine(_rootDirectory, conversionId), true);

    public Stream GetConvertedFile(string conversionId)
    {
        var directory = GetDirectory(conversionId);
        if (!directory.Exists) throw new ConvertedFileGoneException(conversionId);
        try
        {
            return directory.GetFiles().Single().OpenRead();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Equals("Sequence contains no elements")) throw new ConvertedFileGoneException(conversionId);
            throw;
        }
    }
    
    private void EnsureDirectory(string conversionId)
    {
        var directory = Path.Combine(_rootDirectory, conversionId); 
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }
    
}