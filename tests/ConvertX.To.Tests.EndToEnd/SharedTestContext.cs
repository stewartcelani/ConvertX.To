using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MimeTypes.Core;

namespace ConvertX.To.Tests.EndToEnd;

public class SharedTestContext : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    
    public static List<FileInfo> GetSampleFilesForFormat(string format)
    {
        var sampleFileDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            $"../../../../SampleFiles")));
        return sampleFileDirectory
            .GetFiles("*.*", SearchOption.AllDirectories)
            .Where(x => !x.Name.Contains(".converted."))
            .Where(x => x.Extension == $".{format}")
            .ToList();
    }
    
    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var mimeType = MimeTypeMap.GetMimeType(extension);
        return mimeType ?? throw new NullReferenceException(nameof(mimeType));
    }

    private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "ConvertXToEndToEndTest",
            Username = "postgres",
            Password = "wydB191DRtSv"
        })
        .WithPortBinding(5539, 5432)
        .Build();
    
  
}