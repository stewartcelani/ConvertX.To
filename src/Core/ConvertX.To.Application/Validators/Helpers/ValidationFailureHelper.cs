using FluentValidation.Results;

namespace ConvertX.To.Application.Validators.Helpers;

public class ValidationFailureHelper
{
    public static IEnumerable<ValidationFailure> Generate(string paramName, string message)
    {
        return new[]
        {
            new ValidationFailure(paramName, message)
        };
    }
}