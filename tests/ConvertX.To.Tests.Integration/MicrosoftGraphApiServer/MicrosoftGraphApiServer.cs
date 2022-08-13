using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using ConvertX.To.Application.Domain.Settings;
using MimeTypes.Core;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit.Sdk;
using Request = WireMock.RequestBuilders.Request;

namespace ConvertX.To.Tests.Integration.MicrosoftGraphApiServer;

[ExcludeFromCodeCoverage]
public class MicrosoftGraphApiServer : IDisposable
{
    private WireMockServer _server;
    private readonly MsGraphSettings _msGraphSettings;

    public MicrosoftGraphApiServer(MsGraphSettings msGraphSettings)
    {
        _msGraphSettings = msGraphSettings;
    }

    public string Url => _server.Url!;

    public async Task StartAsync()
    {
        _server = WireMockServer.Start();
        await SetupAuthenticationEndpoint();
        SetupDeleteFileAsyncEndpoint();
    }

    private async Task SetupAuthenticationEndpoint()
    {
        var requestUrl = $"{_msGraphSettings.AuthenticationEndpoint}/{_msGraphSettings.TenantId}/oauth2/v2.0/token";

        _server.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingPost())
            .RespondWith(Response.Create()
                .WithBodyAsJson(await new StreamReader(@"Responses\AuthenticationResponse.json").ReadToEndAsync())
                .WithStatusCode(200));
    }

    public async Task SetupUploadFileAsyncEndpoint(string tempFileName, Stream source)
    {
        var extension = Path.GetExtension(tempFileName);
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/root:/*.{extension}:/content";

        _server.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingPut())
            .RespondWith(Response.Create()
                .WithBodyAsJson(GenerateUploadFileResponseBody(tempFileName, source))
                .WithHeader("content-type",
                    "application/json; odata.metadata=minimal; odata.streaming=true; IEEE754Compatible=false; charset=utf-8")
                .WithStatusCode(201));
    }

    public async Task SetupGetFileInTargetFormatAsyncEndpoint(string sampleFileName, string targetFormat)
    {
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/*/content?format={targetFormat}";

        if (targetFormat.Equals("jpg"))
            requestUrl += "&width=1920&height=1080";

        var responseBodyMimeType = SharedTestContext.GetMimeType(targetFormat);

        _server.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(await File.ReadAllBytesAsync(SharedTestContext
                    .GetSampleFile($"{sampleFileName}.converted.{targetFormat}").FullName))
                .WithHeader("content-type", responseBodyMimeType)
                .WithStatusCode(200));
    }

    public void SetupDeleteFileAsyncEndpoint()
    {
        var requestUrl = $"{_msGraphSettings.GraphEndpoint}/*";

        _server.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(204));
    }

    public void Dispose()
    {
        _server.Stop();
        _server.Dispose();
    }

    private static string GenerateUploadFileResponseBody(string tempFileName, Stream source)
    {
        var now = DateTime.UtcNow.ToString("s") + "Z";
        var mimeType = SharedTestContext.GetMimeType(tempFileName);

        return $@"
          {{
            ""@odata.context"": ""https://graph.microsoft.com/beta/$metadata#sites('7pyrnm.sharepoint.com%2Ca0463365-9d85-40df-99e5-8ee8b818c491%2C31643879-48df-4ff0-aa79-7c9a66159779')/drive/items/$entity"",
            ""@microsoft.graph.downloadUrl"": ""https://7pyrnm.sharepoint.com/sites/ConvertX.To/_layouts/15/download.aspx?UniqueId=d93f8e60-d895-4661-bb5f-3285aa5bfb51&Translate=false&tempauth=eyJ0eXAiOiJKV1QiLCJhbGciOiJub25lIn0.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDAvN3B5cm5tLnNoYXJlcG9pbnQuY29tQDUzNWQ1NTljLTczZjctNDAzNC04OWZkLTFhOTRjZTBmOTA1OCIsImlzcyI6IjAwMDAwMDAzLTAwMDAtMGZmMS1jZTAwLTAwMDAwMDAwMDAwMCIsIm5iZiI6IjE2NjAzNTE3MTMiLCJleHAiOiIxNjYwMzU1MzEzIiwiZW5kcG9pbnR1cmwiOiJncjcwajNJQkpmUGthZVlJZ1V6bGt4Qk54R0F0YkRVSnBiNXRUS2N0R0VVPSIsImVuZHBvaW50dXJsTGVuZ3RoIjoiMTM1IiwiaXNsb29wYmFjayI6IlRydWUiLCJjaWQiOiJZbUl4TmpaaE1qY3RNVGxqWVMwMFl6Y3lMVGcxTXpRdFpUSXdPRFZoTmpFM04yTTAiLCJ2ZXIiOiJoYXNoZWRwcm9vZnRva2VuIiwic2l0ZWlkIjoiWVRBME5qTXpOalV0T1dRNE5TMDBNR1JtTFRrNVpUVXRPR1ZsT0dJNE1UaGpORGt4IiwiYXBwX2Rpc3BsYXluYW1lIjoiQ29udmVydFguVG8iLCJuYW1laWQiOiJhMTA0NzgyYy01YTc0LTRiYjAtYmZiZi01MDRlZGU1OWFjOTlANTM1ZDU1OWMtNzNmNy00MDM0LTg5ZmQtMWE5NGNlMGY5MDU4Iiwicm9sZXMiOiJhbGxmaWxlcy53cml0ZSBhbGxmaWxlcy5yZWFkIiwidHQiOiIxIiwidXNlUGVyc2lzdGVudENvb2tpZSI6bnVsbCwiaXBhZGRyIjoiMjAuMTkwLjE0Mi4xNjkifQ.OTg0MlBtK3JJSm0wK08yRXU3QjhMVTZpOEgzVStiZllodWQzZkJrRHdSbz0&ApiVersion=2.0"",
            ""createdDateTime"": ""{now}"",
            ""eTag"": ""\""{{D93F8E60-D895-4661-BB5F-3285AA5BFB51}},2\"""",
            ""id"": ""01NFUP5G3ARY75TFOYMFDLWXZSQWVFX62R"",
            ""lastModifiedDateTime"": ""{now}"",
            ""name"": ""{tempFileName}"",
            ""webUrl"": ""https://7pyrnm.sharepoint.com/sites/ConvertX.To/_layouts/15/Doc.aspx?sourcedoc=%7BD93F8E60-D895-4661-BB5F-3285AA5BFB51%7D&file={tempFileName}&action=default&mobileredirect=true"",
            ""cTag"": ""\""c:{{D93F8E60-D895-4661-BB5F-3285AA5BFB51}},2\"""",
            ""size"": {source.Length},
            ""createdBy"": {{
              ""application"": {{
                ""id"": ""a104782c-5a74-4bb0-bfbf-504ede59ac99"",
                ""displayName"": ""ConvertX.To""
              }},
              ""user"": {{
                ""displayName"": ""SharePoint App""
              }}
            }},
            ""lastModifiedBy"": {{
              ""application"": {{
                ""id"": ""a104782c-5a74-4bb0-bfbf-504ede59ac99"",
                ""displayName"": ""ConvertX.To""
              }},
              ""user"": {{
                ""displayName"": ""SharePoint App""
              }}
            }},
            ""parentReference"": {{
              ""driveType"": ""documentLibrary"",
              ""driveId"": ""b!ZTNGoIWd30CZ5Y7ouBjEkXk4ZDHfSPBPqnl8mmYVl3nGDBxr18ZFQJdG8a6WeezI"",
              ""id"": ""01NFUP5G56Y2GOVW7725BZO354PWSELRRZ"",
              ""path"": ""/drive/root:""
            }},
            ""file"": {{
              ""mimeType"": ""{mimeType}"",
              ""hashes"": {{
                ""quickXorHash"": ""70fCJ3su6yNIgsNO91tOG5anl+U=""
              }}
            }},
            ""fileSystemInfo"": {{
              ""createdDateTime"": ""{now}"",
              ""lastModifiedDateTime"": ""{now}""
            }}
          }}
        ";
    }
}