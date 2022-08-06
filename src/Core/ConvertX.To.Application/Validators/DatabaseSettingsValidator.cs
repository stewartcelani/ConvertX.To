using ConvertX.To.Application.Domain.Settings;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class DatabaseSettingsValidator : AbstractValidator<DatabaseSettings>
{
    public DatabaseSettingsValidator()
    {
        RuleFor(x => x.ConnectionString)
            .Must(s => !string.IsNullOrEmpty(s))
            .MinimumLength(6)
            .WithMessage($"Invalid {nameof(DatabaseSettings.ConnectionString)}");

        RuleFor(x => x.UseInMemoryDatabase)
            .NotNull();

        RuleFor(x => x.EnableSensitiveDataLogging)
            .NotNull();
    }
}