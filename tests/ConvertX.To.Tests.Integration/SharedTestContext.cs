using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bogus;
using ConvertX.To.Application.Converters;
using ConvertX.To.Domain;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Xunit;

namespace ConvertX.To.Tests.Integration;

public class SharedTestContext : IAsyncLifetime
{
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

    public SharedTestContext()
    {
        HttpClient = new HttpClient();
        HttpClient.BaseAddress = new Uri($"{AppUrl}/");
    }

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
        _dockerService.Start();
        await Task.Delay(2000); // Hacky. Needed so tests don't run too early for controllers to hook up. Tried WaitForHttp variations with no success.
    }

    public new async Task DisposeAsync()
    {
        HttpClient.Dispose();
        _dockerService.Dispose();
        await Task.Delay(5000);  // Hacky. Needed for ICompositeService.RemoveAllImages() not to cause tests to fail
    }
    
    public static FileInfo GetSampleFile(string name) => new FileInfo(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), $"../../../../SampleFiles/{name}")));

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