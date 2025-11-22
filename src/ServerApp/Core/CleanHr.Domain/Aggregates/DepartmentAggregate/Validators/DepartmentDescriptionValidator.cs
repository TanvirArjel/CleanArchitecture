using FluentValidation;

namespace CleanHr.Domain.Aggregates.DepartmentAggregate.Validators;

public class DepartmentDescriptionValidator : AbstractValidator<string>
{
    public DepartmentDescriptionValidator()
    {
        RuleFor(description => description)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage("The {PropertyName} is required.")
               .MinimumLength(20)
               .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
               .MaximumLength(200)
               .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.");
    }
}
