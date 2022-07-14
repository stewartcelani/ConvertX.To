using ConvertX.To.Domain.Settings;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class TimeToLiveSettingsValidator :AbstractValidator<TimeToLiveSettings>
{
    public TimeToLiveSettingsValidator()
    {
        RuleFor(x => x.TTL)
            .GreaterThanOrEqualTo(10);
    }
}