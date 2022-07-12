using ConvertX.To.Domain.Settings;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class LocalStorageSettingsValidator : AbstractValidator<LocalStorageSettings>
{
    public LocalStorageSettingsValidator()
    {
        RuleFor(x => x.RootDirectory)
            .Must(s => !string.IsNullOrEmpty(s))
            .WithMessage($"Invalid {nameof(LocalStorageSettings.RootDirectory)}");

    }
}