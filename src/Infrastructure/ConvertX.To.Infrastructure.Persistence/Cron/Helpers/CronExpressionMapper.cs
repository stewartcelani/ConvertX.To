using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Infrastructure.Persistence.Cron.Helpers;

[ExcludeFromCodeCoverage]
public static class CronExpressionHelper
{
    public static string EverySeconds(int seconds)
    {
        ThrowIfOutOfRange(seconds);
        return $"*/{seconds} * * * * *";
    }

    public static string EveryMinutes(int minutes)
    {
        ThrowIfOutOfRange(minutes);
        return $"* */{minutes} * * * *";
    }

    public static string EveryHours(int hours)
    {
        ThrowIfOutOfRange(hours);
        return $"* * */{hours} * * *";
    }

    public static string EveryDays(int days)
    {
        ThrowIfOutOfRange(days);
        return $"* * * */{days} * *";
    }

    private static void ThrowIfOutOfRange(int n)
    {
        if (n is < 1 or > 59) throw new ArgumentOutOfRangeException(nameof(n), n, "Valid range: 1-59");
    }
}