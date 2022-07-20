using ConvertX.To.Domain.Settings;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class ConversionStorageSettingsValidator : AbstractValidator<ConversionStorageSettings>
{
    public ConversionStorageSettingsValidator()
    {
        RuleFor(x => x.RootDirectory)
            .Must(x => !string.IsNullOrEmpty(x));
    }
}