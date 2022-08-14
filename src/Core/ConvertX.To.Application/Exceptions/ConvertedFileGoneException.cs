using System.Diagnostics.CodeAnalysis;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class ConvertedFileGoneException : ConvertXToExceptionBase
{
    public ConvertedFileGoneException(string conversionId) : base(
        $"Converted file with id {conversionId} no longer exists on server.")
    {
    }
}