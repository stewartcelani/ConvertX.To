namespace ConvertX.To.Domain.Settings;

public class DatabaseSettings
{
    public bool EnableSensitiveDataLogging { get; set; }
    public bool UseInMemoryDatabase { get; set; }
    public string ConnectionString { get; set; }
}