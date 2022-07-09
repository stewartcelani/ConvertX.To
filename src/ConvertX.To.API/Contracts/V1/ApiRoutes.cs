namespace ConvertX.To.API.Contracts.V1;

public static class ApiRoutes
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Base = $"{Root}/{Version}";
    
    public static class Files
    {
        public const string Get = Base + "/file/{fileId}";
    }

    public static class Convert
    {
        public const string Post = Base + "/convert/{from}/{to}";
    }
}