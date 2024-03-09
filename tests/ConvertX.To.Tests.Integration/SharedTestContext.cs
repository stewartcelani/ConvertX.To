using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Domain;
using ConvertX.To.Infrastructure.Shared.Services;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MimeTypes.Core;
using Newtonsoft.Json;
using Xunit;

namespace ConvertX.To.Tests.Integration;


public class SharedTestContext : IAsyncLifetime
{
    public const int WireMockServerPort = 51923;

    private static readonly string WireMockServerUrl = $"http://localhost:{WireMockServerPort}";

    public MicrosoftGraphApiServer.MicrosoftGraphApiServer MicrosoftGraphApiServer { get; }

    public static readonly Faker<Conversion> ConversionGenerator = new Faker<Conversion>()
        .RuleFor(x => x.Id, faker => faker.Random.Guid())
        .RuleFor(x => x.ConvertedFormat, GetRandomFormat)
        .RuleFor(x => x.ConvertedMegabytes, faker => faker.Random.Decimal(0.0m, 10.0m))
        .RuleFor(x => x.SourceFormat, GetRandomFormat)
        .RuleFor(x => x.SourceMegabytes, faker => faker.Random.Decimal(0.0m, 10.0m))
        .RuleFor(x => x.TargetFormat, GetRandomFormat)
        .RuleFor(x => x.DateRequestReceived,
            faker => DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(faker.Random.Int(45, 90))))
        .RuleFor(x => x.DateRequestCompleted,
            faker => DateTimeOffset.Now.Subtract(TimeSpan.FromSeconds(faker.Random.Int(5, 35))));

    public SharedTestContext()
    {
        var appSettingsSecretFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
            "../../../MicrosoftGraphApiServer/MsGraphSettings.secret.json"));
        var msGraphSettings = JsonConvert.DeserializeObject<MsGraphSettings>(File.ReadAllText(appSettingsSecretFile)) ??
                              throw new NullReferenceException(nameof(appSettingsSecretFile));
        msGraphSettings.AuthenticationEndpoint = WireMockServerUrl;
        msGraphSettings.GraphEndpoint = WireMockServerUrl;
        MicrosoftGraphApiServer = new MicrosoftGraphApiServer.MicrosoftGraphApiServer(msGraphSettings);
    }

    public async Task InitializeAsync()
    {
        await MicrosoftGraphApiServer.StartAsync();
        await _dbContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        MicrosoftGraphApiServer.Dispose();
        await _dbContainer.DisposeAsync();
    }


    public static FileInfo GetSampleFile(string name)
    {
        return new FileInfo(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
            $"../../../../SampleFiles/{name}")));
    }

    public static List<FileInfo> GetSampleFiles()
    {
        var sampleFileDirectory = new DirectoryInfo(Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(),
            $"../../../../SampleFiles")));
        return sampleFileDirectory
            .GetFiles("*.*", SearchOption.AllDirectories)
            .Where(x => !x.Name.Contains(".converted."))
            .ToList();
    }

    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var mimeType = MimeTypeMap.GetMimeType(extension);
        return mimeType ?? throw new NullReferenceException(nameof(mimeType));
    }

    private static List<string>? _formats;

    private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
        .WithDatabase(new PostgreSqlTestcontainerConfiguration
        {
            Database = "ConvertXToTest",
            Username = "postgres",
            Password = "rydA191NNtUv"
        })
        .WithPortBinding(5439, 5432)
        .Build();

    private static List<string> Formats
    {
        get
        {
            if (_formats is not null) return _formats;

            var supportedConversions = ConversionEngine.GetSupportedConversions();

            var formats = new List<string>();

            foreach (var key in supportedConversions.SourceFormatTo.Keys.Where(key => !formats.Contains(key)))
                formats.Add(key);
            foreach (var key in supportedConversions.TargetFormatFrom.Keys.Where(key => !formats.Contains(key)))
                formats.Add(key);

            _formats = formats;
            return _formats;
        }
    }

    private static string GetRandomFormat()
    {
        var index = new Random().Next(Formats.Count);
        return Formats[index];
    }
}