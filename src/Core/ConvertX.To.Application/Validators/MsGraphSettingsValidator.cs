using ConvertX.To.Application.Domain.Settings;
using ConvertX.To.Application.Validators.Mappers;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class MsGraphSettingsValidator : AbstractValidator<MsGraphSettings>
{
    public MsGraphSettingsValidator()
    {
        RuleFor(x => x.TenantId)
            .Must(s => !string.IsNullOrEmpty(s))
            .Matches(RegexMapper.Guid)
            .WithMessage("Invalid guid.");

        RuleFor(x => x.ClientId)
            .Must(s => !string.IsNullOrEmpty(s))
            .Matches(RegexMapper.Guid)
            .WithMessage("Invalid guid.");

        RuleFor(x => x.ClientSecret)
            .Must(s => !string.IsNullOrEmpty(s))
            .MinimumLength(10)
            .WithMessage("Invalid client secret.");

        RuleFor(x => x.Scope)
            .Must(s => !string.IsNullOrEmpty(s))
            .Must(s => Uri.TryCreate(s, UriKind.Absolute, out _))
            .WithMessage("Invalid uri.");

        RuleFor(x => x.AuthenticationEndpoint)
            .Must(s => !string.IsNullOrEmpty(s))
            .Must(s => Uri.TryCreate(s, UriKind.Absolute, out _))
            .WithMessage("Invalid uri.");

        RuleFor(x => x.GraphEndpoint)
            .Must(s => !string.IsNullOrEmpty(s))
            .Must(s => Uri.TryCreate(s, UriKind.Absolute, out _))
            .WithMessage("Invalid uri.");

    }
}