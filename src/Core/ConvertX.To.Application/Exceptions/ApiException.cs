using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using FluentValidation.Results;

namespace ConvertX.To.Application.Exceptions;

/// <summary>
///     Re-purposing FluentValidation's ValidationException but for neatly showing 500 errors via middleware
/// </summary>
[ExcludeFromCodeCoverage]
[Serializable]
public class ApiException : Exception
{
    /// <summary>
    ///     Creates a new ApiException
    /// </summary>
    /// <param name="message"></param>
    public ApiException(string message) : this(message, Enumerable.Empty<ValidationFailure>())
    {
    }

    /// <summary>
    ///     Creates a new ApiException
    /// </summary>
    /// <param name="message"></param>
    /// <param name="errors"></param>
    public ApiException(string message, IEnumerable<ValidationFailure> errors) : base(message)
    {
        Errors = errors;
    }

    /// <summary>
    ///     Creates a new ApiException
    /// </summary>
    /// <param name="message"></param>
    /// <param name="errors"></param>
    /// <param name="appendDefaultMessage">appends default validation error message to message</param>
    public ApiException(string message, IEnumerable<ValidationFailure> errors, bool appendDefaultMessage)
        : base(appendDefaultMessage ? $"{message} {BuildErrorMessage(errors)}" : message)
    {
        Errors = errors;
    }

    /// <summary>
    ///     Creates a new ApiException
    /// </summary>
    /// <param name="errors"></param>
    public ApiException(IEnumerable<ValidationFailure> errors) : base(BuildErrorMessage(errors))
    {
        Errors = errors;
    }

    public ApiException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
        Errors = info.GetValue("errors", typeof(IEnumerable<ValidationFailure>)) as IEnumerable<ValidationFailure>;
    }

    /// <summary>
    ///     Validation errors
    /// </summary>
    public IEnumerable<ValidationFailure> Errors { get; }

    private static string BuildErrorMessage(IEnumerable<ValidationFailure> errors)
    {
        var arr = errors.Select(x =>
            $"{Environment.NewLine} -- {x.PropertyName}: {x.ErrorMessage} Severity: {x.Severity.ToString()}");
        return "Validation failed: " + string.Join(string.Empty, arr);
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null) throw new ArgumentNullException("info");

        info.AddValue("errors", Errors);
        base.GetObjectData(info, context);
    }
}