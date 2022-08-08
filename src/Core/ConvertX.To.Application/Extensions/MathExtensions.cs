namespace ConvertX.To.Application.Extensions;

public static class ConvertXMathExtensions
{
    public static decimal ToMegabytes(this long i)
    {
        return (decimal)Math.Round(i / 1024f / 1024f, 4);
    }
}