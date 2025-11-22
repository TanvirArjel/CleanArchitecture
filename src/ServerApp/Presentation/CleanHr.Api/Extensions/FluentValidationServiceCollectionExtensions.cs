using FluentValidation;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace CleanHr.Api;

internal static class FluentValidationServiceCollectionExtensions
{
    public static void AddFluentValidation(this IServiceCollection services)
    {
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        // Make sure this is from SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
        // Otherwise Async validation would not work.
        services.AddFluentValidationAutoValidation();
    }
}
