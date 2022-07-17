﻿using ConvertX.To.Application.Interfaces;
using ConvertX.To.Domain.Settings;

namespace ConvertX.To.Infrastructure.Shared.Services;

public class LocalFileService : IFileService
{
    private readonly FileServiceSettings _fileServiceSettings;

    public LocalFileService(FileServiceSettings fileServiceSettings)
    {
        _fileServiceSettings = fileServiceSettings;
    }

    public async Task SaveFileAsync(string path, Stream stream)
    {
        EnsureDirectory(path);
        await using var fileStream = new FileStream(CombinePath(path), FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await stream.DisposeAsync();
        await fileStream.DisposeAsync();
    }

    public DirectoryInfo GetDirectory(string path) => new (CombinePath(path));

    public DirectoryInfo GetRootDirectory()
    {
        if (string.IsNullOrEmpty(_fileServiceSettings.RootDirectory))
            throw new NullReferenceException(_fileServiceSettings.RootDirectory);
        return GetDirectory(_fileServiceSettings.RootDirectory);
    }

    public void DeleteDirectory(string path) => Directory.Delete(CombinePath(path), true);

    public Stream GetFile(string path) => File.OpenRead(CombinePath(path));
    
    /// <summary>
    /// Will ensure the directory for a full file path is created
    /// </summary>
    /// <param name="path">Full file path including fileName.ext</param>
    private void EnsureDirectory(string path)
    {
        var directory = Path.GetDirectoryName(CombinePath(path)); 
        if (directory is not null && !Directory.Exists(directory)) Directory.CreateDirectory(directory);
    }

    private string CombinePath(string path) => path.Equals(_fileServiceSettings.RootDirectory) ? path : Path.Combine(_fileServiceSettings.RootDirectory, path);

}