using FluentValidation;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class EmployeeNameValidator : AbstractValidator<string>
{
    public EmployeeNameValidator(string fieldName)
    {
        RuleFor(name => name)
            .NotEmpty()
            .WithMessage($"The {fieldName} is required.")
            .MinimumLength(2)
            .WithMessage($"The {fieldName} must be at least 2 characters.")
            .MaximumLength(50)
            .WithMessage($"The {fieldName} can't be more than 50 characters long.");
    }
}
