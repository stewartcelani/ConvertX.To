namespace ConvertX.To.Infrastructure.Persistence.Cron.Helpers;

public static class CronExpressionHelper
{
    public static string EverySeconds(int seconds)
    {
        if (seconds is < 1 or > 59) throw new ArgumentOutOfRangeException(nameof(seconds), seconds, "Valid range: 1-59");

        return $"*/{seconds} * * * * *";
    }
    
    public static string EveryMinutes(int hours)
    {
        if (hours is < 1 or > 59) throw new ArgumentOutOfRangeException(nameof(hours), hours, "Valid range: 1-59");

        return $"* */{hours} * * * *";
    }
    
    public static string EveryHours(int hours)
    {
        if (hours is < 1 or > 59) throw new ArgumentOutOfRangeException(nameof(hours), hours, "Valid range: 1-59");

        return $"* * */{hours} * * *";
    }
    
    public static string EveryDays(int days)
    {
        if (days is < 1 or > 59) throw new ArgumentOutOfRangeException(nameof(days), days, "Valid range: 1-59");

        return $"* * * */{days} * *";
    }
}