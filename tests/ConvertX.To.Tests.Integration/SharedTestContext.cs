using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Domain;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using MimeTypes.Core;
using Newtonsoft.Json;
using Xunit;


namespace ConvertX.To.Tests.Integration;

[ExcludeFromCodeCoverage]
public class SharedTestContext : IAsyncLifetime
{
    public MicrosoftGraphApiServer.MicrosoftGraphApiServer MicrosoftGraphApiServer { get; }
    
    public SharedTestContext()
    {
        HttpClient = new HttpClient();
        HttpClient.BaseAddress = new Uri($"{AppUrl}/");

        var appSettingsSecretFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),
            "../../../MicrosoftGraphApiServer/MsGraphSettings.secret.json"));
        var msGraphSettings = JsonConvert.DeserializeObject<MsGraphSettings>(File.ReadAllText(appSettingsSecretFile)) ??
                               throw new NullReferenceException(nameof(appSettingsSecretFile));
        MicrosoftGraphApiServer = new MicrosoftGraphApiServer.MicrosoftGraphApiServer(msGraphSettings);
    }
    
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

    public const string AppUrl = "https://localhost:7780";


    private static readonly string DockerComposeFile =
        Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../../docker-compose.integration.yml"));

    public readonly HttpClient HttpClient;
    
    private readonly ICompositeService _dockerService = new Builder()
        .UseContainer()
        .UseCompose()
        .FromFile(DockerComposeFile)
        .RemoveOrphans()
        .ForceBuild()
        .ForceRecreate() // This will ensure the database is recreated every time tests are launched
        .RemoveAllImages()
        .Build();

    public async Task InitializeAsync()
    {
        await MicrosoftGraphApiServer.StartAsync();
        _dockerService.Start();
        await Task.Delay(3000); // Hacky. Needed so tests don't run too early for controllers to hook up. Tried WaitForHttp variations with no success.
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
        _dockerService.Dispose();
        MicrosoftGraphApiServer.Dispose();
        await Task.Delay(6000);  // Hacky. Needed for ICompositeService.RemoveAllImages() not to cause tests to fail
    }
    
    public static FileInfo GetSampleFile(string name) => new FileInfo(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"../../../../SampleFiles/{name}")));
    
    public static string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName);
        var mimeType = MimeTypeMap.GetMimeType(extension);
        return mimeType ?? throw new NullReferenceException(nameof(mimeType));
    }

    private static string GetRandomFormat()
    {
        var index = new Random().Next(Formats.Count);
        return Formats[index];
    }
    
    private static List<string>? _formats;
    private static List<string> Formats
    {
        get
        {
            if (_formats is not null) return _formats;

            var supportedConversions = ConversionEngine.GetSupportedConversions();

            var formats = new List<string>();
            
            foreach (var key in supportedConversions.SourceFormatTo.Keys.Where(key => !formats.Contains(key)))
            {
                formats.Add(key);
            }
            foreach (var key in supportedConversions.TargetFormatFrom.Keys.Where(key => !formats.Contains(key)))
            {
                formats.Add(key);
            }

            _formats = formats;
            return _formats;
        }
    }
}