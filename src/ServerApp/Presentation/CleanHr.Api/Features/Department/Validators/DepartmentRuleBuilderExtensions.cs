using CleanHr.Api.Features.Department.Models;
using FluentValidation;

namespace CleanHr.Api;

public static class DepartmentRuleBuilderExtensions
{
    public static IRuleBuilderOptions<T, string> ValidName<T>(
        this IRuleBuilder<T, string> rule)
        where T : DepartmentBaseModel
    {
        return rule.NotEmpty()
           .WithMessage("The {PropertyName} is required.")
           .MinimumLength(2)
           .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
           .MaximumLength(20)
           .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");
    }
}
