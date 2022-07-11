namespace ConvertX.To.Domain.Settings;

public class GraphFileServiceSettings
{
    public string TenantId { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string Scope { get; set; }
    public string AuthenticationEndpoint { get; set; }
    public string GraphEndpoint { get; set; }
}