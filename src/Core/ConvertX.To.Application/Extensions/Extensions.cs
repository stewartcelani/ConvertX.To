namespace ConvertX.To.Application.Extensions;

public static class Extensions
{
    public static string Proper(this string s)
    {
        return s.First().ToString().ToUpper() + s.ToLower()[1..];
    }
    
    public static decimal ToMegabytes(this long i) => (decimal)Math.Round((((i) / 1024f) / 1024f), 2);
}