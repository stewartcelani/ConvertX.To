namespace ConvertX.To.API.Contracts.V1;

public static class ApiRoutesV1
{
    private const string Root = "api";
    private const string Version = "v1";
    private const string Base = $"/{Root}/{Version}";

    private static string ReplaceConversionId(this string s, Guid conversionId)
    {
        return s.Replace("{conversionId}", conversionId.ToString());
    }

    public static class Files
    {
        public static class Get
        {
            public const string Url = Base + "/file/{conversionId}";

            public static string UrlFor(Guid conversionId)
            {
                return Url.ReplaceConversionId(conversionId);
            }
        }

        public static class Delete
        {
            public const string Url = Base + "/file/{conversionId}";

            public static string UrlFor(Guid conversionId)
            {
                return Url.ReplaceConversionId(conversionId);
            }
        }
    }

    public static class Convert
    {
        public static class Get
        {
            public const string Url = Base + "/convert";
        }

        public static class Post
        {
            public const string Url = Base + "/convert/{targetFormat}";

            public static string UrlFor(string targetFormat)
            {
                return Url.Replace("{targetFormat}", targetFormat);
            }
        }
    }
}