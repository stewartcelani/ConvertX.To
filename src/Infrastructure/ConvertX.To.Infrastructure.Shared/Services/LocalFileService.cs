using ConvertX.To.Application.Interfaces;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class LocalFileService : IFileService
{
    public async Task SaveFileAsync(string path, Stream stream)
    {
        EnsureDirectory(path);
        await using var fileStream = new FileStream(path, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
        await fileStream.DisposeAsync();
    }

    private static void EnsureDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (directory is not null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }
}