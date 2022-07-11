using ConvertX.To.Domain.Settings;
using FluentValidation;

namespace ConvertX.To.Application.Validators;

public class FileServiceSettingsValidator : AbstractValidator<FileServiceSettings>
{
    public FileServiceSettingsValidator()
    {
        RuleFor(x => x.RootDirectory)
            .Must(s => !string.IsNullOrEmpty(s))
            .WithMessage($"Invalid {nameof(FileServiceSettings.RootDirectory)}");

    }
}