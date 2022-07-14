namespace ConvertX.To.ConsoleUI.API.Settings;

public class WaitAndRetryConfig
{
    public int RetryAttempts { get; set; }
    public int RetrySeconds { get; set; }
    public int TimeoutSeconds { get; set; }
}