using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Bogus;
using ConvertX.To.Application.Converters;
using ConvertX.To.Application.Domain;
using ConvertX.To.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using NSubstitute;

namespace ConvertX.To.Infrastructure.Shared.Tests.Unit;

[ExcludeFromCodeCoverage]
public class SharedTestContext
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

    public static IFormFile GenerateFormFile(string fileName, string? content)
    {
        var bytes = Array.Empty<byte>();
        if (!string.IsNullOrEmpty(content)) bytes = Encoding.UTF8.GetBytes(content); 

        return new FormFile(
            baseStream: new MemoryStream(bytes),
            baseStreamOffset: 0,
            length: bytes.Length,
            name: "Data",
            fileName: fileName
        );
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