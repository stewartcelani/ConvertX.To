using System.Diagnostics.CodeAnalysis;
using ConvertX.To.Application.Exceptions;

namespace ConvertX.To.Application.Exceptions;

[ExcludeFromCodeCoverage]
public class ConvertedFileGoneException : ConvertXToExceptionBase, IBusinessException
{
    public ConvertedFileGoneException(string conversionId) : base($"Converted file with id {conversionId} no longer exists on server.")
    {
    }
}