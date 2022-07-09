using ConvertX.To.API.Contracts.V1.Requests;
using FluentValidation;

namespace ConvertX.To.API.Validators;

public class UploadFileRequestValidator : AbstractValidator<UploadFileRequest>
{
    public UploadFileRequestValidator()
    {
        RuleFor(x => x.ConvertTo)
            .NotNull()
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(6);

        RuleFor(x => x.FileId)
            .NotNull()
            .NotEmpty()
            .Matches(RegexHelper.Guid)
            .WithMessage("Must be a valid Guid.");

        RuleFor(x => x.SessionId)
            .NotNull()
            .NotEmpty()
            .Matches(RegexHelper.Guid)
            .WithMessage("Must be a valid Guid.");

    }
}