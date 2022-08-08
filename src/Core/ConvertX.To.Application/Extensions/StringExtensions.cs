namespace ConvertX.To.Application.Extensions;

public static class ConvertXStringExtensions
{
    public static string Proper(this string s)
    {
        return s.First().ToString().ToUpper() + s.ToLower()[1..];
    }
}