using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Domain.External.MicrosoftGraph.Responses;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace ConvertX.To.Tests.Integration.MicrosoftGraphApiServer;

[ExcludeFromCodeCoverage]
public class MicrosoftGraphApiServer : IDisposable
{
    public MsGraphSettings MsGraphSettings { get; }
    public WireMockServer? Server { get; set; }
    public string Url => Server?.Urls.First() ?? "";

    public MicrosoftGraphApiServer(MsGraphSettings msGraphSettings)
    {
        MsGraphSettings = msGraphSettings;
    }

    public async Task StartAsync()
    {
        Server = WireMockServer.Start(SharedTestContext.WireMockServerPort);
        await Task.Delay(1000);
        SetupPingEndpoint();
        SetupAuthenticationEndpoint();
        SetupDeleteFileAsyncEndpoint();
    }

    private void SetupPingEndpoint()
    {
        Server!.Given(Request.Create()
                .WithPath("/ping")
                .UsingGet())
            .RespondWith(Response.Create().WithStatusCode(200));
    }

    public void Dispose()
    {
        Server?.Stop();
        Server?.Dispose();
    }

    private void SetupAuthenticationEndpoint()
    {
        var requestUrl = $"{MsGraphSettings.AuthenticationEndpoint}/{MsGraphSettings.TenantId}/oauth2/v2.0/token";

        var authenticationResponse = new AuthenticationResponse
        {
            AccessToken =
                "eyJ0eXAiOiJKV1QiLCJub25jZSI6IjM4amdMYjc3NTZFZUJiUlEtTjFSb0xOSzFrbW5QWWk3Wnd3aUVOaG9RM0UiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiJodHRwczovL2dyYXBoLm1pY3Jvc29mdC5jb20iLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC81MzVkNTU5Yy03M2Y3LTQwMzQtODlmZC0xYTk0Y2UwZjkwNTgvIiwiaWF0IjoxNjYwMzUwODYxLCJuYmYiOjE2NjAzNTA4NjEsImV4cCI6MTY2MDM1NDc2MSwiYWlvIjoiRTJaZ1lEQTlJbnJqQnU4RUR3c1ZuLzhHNXJNTUFBPT0iLCJhcHBfZGlzcGxheW5hbWUiOiJDb252ZXJ0WC5UbyIsImFwcGlkIjoiYTEwNDc4MmMtNWE3NC00YmIwLWJmYmYtNTA0ZWRlNTlhYzk5IiwiYXBwaWRhY3IiOiIxIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNTM1ZDU1OWMtNzNmNy00MDM0LTg5ZmQtMWE5NGNlMGY5MDU4LyIsImlkdHlwIjoiYXBwIiwib2lkIjoiM2ZjODE4M2EtZDc4My00ODk2LWE3OTUtYTVhZDc1N2QwZjRlIiwicmgiOiIwLkFYVUFuRlZkVV9kek5FQ0pfUnFVemctUVdBTUFBQUFBQUFBQXdBQUFBQUFBQUFCMUFBQS4iLCJyb2xlcyI6WyJGaWxlcy5SZWFkV3JpdGUuQWxsIiwiRmlsZXMuUmVhZC5BbGwiXSwic3ViIjoiM2ZjODE4M2EtZDc4My00ODk2LWE3OTUtYTVhZDc1N2QwZjRlIiwidGVuYW50X3JlZ2lvbl9zY29wZSI6Ik5BIiwidGlkIjoiNTM1ZDU1OWMtNzNmNy00MDM0LTg5ZmQtMWE5NGNlMGY5MDU4IiwidXRpIjoiaHlKWkxNM1lMVWlQeldkQWg2clVBQSIsInZlciI6IjEuMCIsIndpZHMiOlsiMDk5N2ExZDAtMGQxZC00YWNiLWI0MDgtZDVjYTczMTIxZTkwIl0sInhtc190Y2R0IjoxNjU2NjIwMzUxfQ.nDwfWKXWl73CgmMP5T3jvSR7HxJzcqoJLI2qB9Lp6Ha489czs4oFFbMmYSQ7Zm8pIWfe2W3-I6JjlkIagCTGLoyA1M6BdbfPgRKbtf8kl7869ElUTOz7N8qkBdEzoId4IclzfZAfj_Gy04MW-IfAtLiKplJhq6j4qgLAn1NSQEdcrdstTPhFp75jb1cRN64amKd4FmfQGvGhQrP2WpSQxn4UNdTVuzZ7t0-M4Plp-zEUIXIa4JSkvmKlikWbtuIca74vIEtVEbUZvubf619UeNJF0c-EqSlk8u1ocZOKIakYhXx9kyXWKNSm2vHjlkKGqusDV6K3Cfzlz_BHpPzdDQ",
            ExpiresIn = 3599,
            ExtExpiresIn = 3599,
            TokenType = "Bearer"
        };

        var serializedAuthenticationResponse = JsonSerializer.Serialize(authenticationResponse);

        Server!.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingPost())
            .RespondWith(Response.Create()
                .WithBody(serializedAuthenticationResponse)
                .WithStatusCode(200));
    }

    public void SetupUploadFileAsyncEndpoint(string tempFileName, Stream source)
    {
        var extension = Path.GetExtension(tempFileName);
        var requestUrl = $"{MsGraphSettings.GraphEndpoint}/root:/*{extension}:/content";

        Server!.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingPut())
            .RespondWith(Response.Create()
                .WithBody(GenerateUploadFileResponseBody(tempFileName, source))
                .WithHeader("content-type",
                    "application/json; odata.metadata=minimal; odata.streaming=true; IEEE754Compatible=false; charset=utf-8")
                .WithStatusCode(201));
    }

    public async Task SetupGetFileInTargetFormatAsyncEndpoint(string sampleFileName, string targetFormat)
    {
        var requestUrl = $"{MsGraphSettings.GraphEndpoint}/*/content?format={targetFormat}";

        if (targetFormat.Equals("jpg"))
            requestUrl += "&width=1920&height=1080";

        var responseBodyMimeType = SharedTestContext.GetMimeType(targetFormat);

        Server!.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingGet())
            .RespondWith(Response.Create()
                .WithBody(await File.ReadAllBytesAsync(SharedTestContext
                    .GetSampleFile($"{sampleFileName}.converted.{targetFormat}").FullName))
                .WithHeader("content-type", responseBodyMimeType)
                .WithStatusCode(200));
    }

    private void SetupDeleteFileAsyncEndpoint()
    {
        var requestUrl = $"{MsGraphSettings.GraphEndpoint}/*";

        Server!.Given(Request.Create()
                .WithUrl(requestUrl)
                .UsingDelete())
            .RespondWith(Response.Create()
                .WithStatusCode(204));
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