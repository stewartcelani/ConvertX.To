namespace ConvertX.To.API.Contracts.V1;

public static class ApiRoutesV1
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Base = $"{Root}/{Version}";
    
    public static class Files
    {
        public const string Get = Base + "/file/{conversionId}";
    }

    public static class Convert
    {
        public const string Get = Base + "/convert";
        public const string Post = Base + "/convert/{targetFormat}";
    }
}