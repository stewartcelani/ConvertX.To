using System.IO.Compression;
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

    public async Task<Stream> ZipFilesAsync(IEnumerable<FileInfo> files)
    {
        var zipStream = new MemoryStream();

        using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = zip.CreateEntry(file.Name);
                await using var entryStream = entry.Open();
                await using var fileStream = file.OpenRead();
                await fileStream.CopyToAsync(entryStream);
            }
        }

        zipStream.Position = 0;
        return zipStream;
    }

    private static void EnsureDirectory(string path)
    {
        var directory = Path.GetDirectoryName(path);
        if (directory is not null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }
}