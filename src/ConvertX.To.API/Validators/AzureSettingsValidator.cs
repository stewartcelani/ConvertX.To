using ConvertX.To.API.Settings;
using FluentValidation;

namespace ConvertX.To.API.Validators;

public class AzureSettingsValidator : AbstractValidator<AzureSettings>
{
    public AzureSettingsValidator()
    {
        RuleFor(x => x.TenantId)
            .Must(s => !string.IsNullOrEmpty(s))
            .Matches(RegexMapper.Guid);

        RuleFor(x => x.ClientId)
            .Must(s => !string.IsNullOrEmpty(s))
            .Matches(RegexMapper.Guid);

        RuleFor(x => x.ClientSecret)
            .Must(s => !string.IsNullOrEmpty(s))
            .MinimumLength(10);
        
        RuleFor(x => x.Scope)
            .Must(s => !string.IsNullOrEmpty(s))
            .Must(s => Uri.TryCreate(s, UriKind.Absolute, out _));

        RuleFor(x => x.AuthenticationEndpoint)
            .Must(s => !string.IsNullOrEmpty(s))
            .Must(s => Uri.TryCreate(s, UriKind.Absolute, out _));

        RuleFor(x => x.GraphEndpoint)
            .Must(s => !string.IsNullOrEmpty(s))
            .Must(s => Uri.TryCreate(s, UriKind.Absolute, out _));

    }
}