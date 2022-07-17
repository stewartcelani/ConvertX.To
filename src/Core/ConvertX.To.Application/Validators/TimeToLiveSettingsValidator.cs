using ConvertX.To.Domain.Settings;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class ConversionLifecycleManagerSettingsValidator :AbstractValidator<ConversionLifecycleManagerSettings>
{
    public ConversionLifecycleManagerSettingsValidator()
    {
        RuleFor(x => x.TimeToLiveInMinutes)
            .GreaterThanOrEqualTo(5);
    }
}