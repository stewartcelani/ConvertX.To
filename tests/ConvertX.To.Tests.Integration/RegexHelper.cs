using System;
using System.Text.RegularExpressions;

namespace ConvertX.To.Tests.Integration;

public class RegexHelper
{
    public static Regex Guid => new (@"(^([0-9A-Fa-f]{8}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{4}[-][0-9A-Fa-f]{12})$)", RegexOptions.IgnoreCase);
}