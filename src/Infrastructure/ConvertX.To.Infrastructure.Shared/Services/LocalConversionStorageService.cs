using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Exceptions;
using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class LocalConversionStorageService : IConversionStorageService
{
    private readonly string _rootDirectory;

    public LocalConversionStorageService(ConversionStorageSettings conversionStorageSettings)
    {
        _rootDirectory = conversionStorageSettings.RootDirectory ??
                         throw new NullReferenceException(nameof(conversionStorageSettings.RootDirectory));
    }

    public async Task SaveConversionAsync(Guid conversionId, string convertedFileName, Stream stream)
    {
        var id = conversionId.ToString();
        EnsureDirectory(id);
        await using var fileStream =
            new FileStream(Path.Combine(_rootDirectory, id, convertedFileName), FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
        await fileStream.DisposeAsync();
    }

    public DirectoryInfo GetDirectory(string conversionId)
    {
        return new DirectoryInfo(Path.Combine(_rootDirectory, conversionId));
    }

    public DirectoryInfo GetRootDirectory()
    {
        return GetDirectory(_rootDirectory);
    }

    public void DeleteConvertedFile(Guid conversionId)
    {
        Directory.Delete(Path.Combine(_rootDirectory, conversionId.ToString()), true);
    }

    public Stream GetConvertedFile(Guid conversionId)
    {
        var id = conversionId.ToString();
        var directory = GetDirectory(id);
        if (!directory.Exists) throw new ConvertedFileGoneException(id);
        try
        {
            return directory.GetFiles().Single().OpenRead();
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Equals("Sequence contains no elements")) throw new ConvertedFileGoneException(id);
            throw;
        }
    }

    private void EnsureDirectory(string conversionId)
    {
        var directory = Path.Combine(_rootDirectory, conversionId);
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }
}