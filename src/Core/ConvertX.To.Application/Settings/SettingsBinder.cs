using FluentValidation;
using Microsoft.Extensions.Configuration;

namespace ConvertX.To.Application.Settings;

public static class SettingsBinder
{
    public static TSettings Bind<TSettings>(IConfiguration configuration) where TSettings : class
    {
        var settings = Activator.CreateInstance<TSettings>();
        configuration.GetSection(settings.GetType().Name).Bind(settings);
        return settings;
    }

    public static TSettings BindAndValidate<TSettings, TValidator>(IConfiguration configuration) where TSettings : class
        where TValidator : AbstractValidator<TSettings>
    {
        var settings = Bind<TSettings>(configuration);
        var validator = Activator.CreateInstance<TValidator>();
        var validationResult = validator.Validate(settings);
        if (!validationResult.IsValid)
            throw new ValidationException($"Failed binding {settings.GetType().Name} using ConfigurationBinder.Bind");
        return settings;
    }
}