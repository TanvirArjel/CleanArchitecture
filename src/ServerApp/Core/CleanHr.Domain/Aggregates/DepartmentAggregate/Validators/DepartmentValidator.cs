using FluentValidation;

namespace CleanHr.Domain.Aggregates.DepartmentAggregate.Validators;

public class DepartmentValidator : AbstractValidator<Department>
{
    public DepartmentValidator()
    {
        // Add validation rules here
        RuleFor(d => d.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("The {PropertyName} is required.")
                .MinimumLength(2)
                .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
                .MaximumLength(20)
                .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
                .WithMessage("The DepartmentName is already existent.");

        RuleFor(d => d.Description)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage("The {PropertyName} is required.")
               .MinimumLength(20)
               .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
               .MaximumLength(200)
               .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");

    }
}
