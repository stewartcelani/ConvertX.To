using ConvertX.To.ConsoleUI.Interfaces;
using Refit;
using MimeTypes.Core;

var apiClient = RestService.For<IConvertXToApi>(new HttpClient
{
    BaseAddress = new Uri("https://localhost:8001"),
    Timeout = TimeSpan.FromSeconds(60 * 5)
});

var directory = new DirectoryInfo(@"C:\dev\convertx.to\sample_files");


var supportedConversionsResponse = await apiClient.GetSupportedConversionsAsync();
if (!supportedConversionsResponse.IsSuccessStatusCode) throw new Exception("E0");
var supportedConversions = supportedConversionsResponse.Content;

var files = directory.GetFiles("*.*", SearchOption.AllDirectories).Where(x => !x.Name.Contains("ConvertX.To")).ToList();

foreach (var fileInfo in files)
{
    var sourceFormat = fileInfo.Extension.Replace(".", "");
    if (!supportedConversions.SourceFormatTo.ContainsKey(sourceFormat)) return;
    var supportedTargetFormats = supportedConversions.SourceFormatTo[sourceFormat];
    foreach (var targetFormat in supportedTargetFormats)
    {
        Console.WriteLine($"Converting {fileInfo.Name} to {targetFormat}");
        var conversionResponse = await apiClient.ConvertAsync(targetFormat, new StreamPart(fileInfo.OpenRead(), fileInfo.Name, MimeTypeMap.GetMimeType(fileInfo.Extension)));
        if (!conversionResponse.IsSuccessStatusCode) 
            throw new Exception("E1");
        var conversion = conversionResponse.Content!;
        var downloadedFile = await apiClient.DownloadConvertedFileAsync(conversion.Id);
        if (!downloadedFile.IsSuccessStatusCode) 
            throw new Exception("E2");
        await using var convertedStream = await downloadedFile.Content.ReadAsStreamAsync();
        var fileName = conversion.TargetFormat == conversion.ConvertedFileExtension
            ? $"{conversion.SourceFileName}.ConvertX.To.{conversion.TargetFormat}"
            : $"{conversion.SourceFileName}.ConvertX.To.{conversion.TargetFormat}.{conversion.ConvertedFileExtension}";
        await using var fileStream = new FileStream(Path.Combine(fileInfo.DirectoryName!, fileName), FileMode.Create);
        await convertedStream.CopyToAsync(fileStream);
        await convertedStream.DisposeAsync();
        await fileStream.DisposeAsync();
        Console.WriteLine($"{fileInfo.Name} successfully converted to {targetFormat}!");
        Console.WriteLine();
    }
}

Console.WriteLine("End Program");