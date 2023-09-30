using CleanHr.Api.Features.Department.Models;
using FluentValidation;

namespace CleanHr.Api.Features.Department.Validators;

public abstract class DepartmentBaseModelValidator<T> : AbstractValidator<T>
    where T : DepartmentBaseModel
{
    protected DepartmentBaseModelValidator()
    {
        RuleFor(d => d.Name)
               .NotEmpty().WithMessage("The {PropertyName} is required.")
               .MaximumLength(20).WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
               .MinimumLength(2).WithMessage("The {PropertyName} must be at least {MinLength} characters.");

        RuleFor(d => d.Description)
               .NotEmpty().WithMessage("The {PropertyName} is required.")
               .MaximumLength(200).WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
               .MinimumLength(20).WithMessage("The {PropertyName} must be at least {MinLength} characters.");
    }
}
