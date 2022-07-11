namespace ConvertX.To.Application.Extensions;

public static class StringExtensions
{
    public static string Proper(this string s)
    {
        return s.First().ToString().ToUpper() + s.ToLower()[1..];
    }
}