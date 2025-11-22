using FluentValidation;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

/// <summary>
/// Validator for individual name components (FirstName, LastName).
/// Matches the validation rules from RegistrationModelValidator.
/// </summary>
public class UserNameComponentValidator : AbstractValidator<string>
{
    public UserNameComponentValidator(string fieldName)
    {
        RuleFor(name => name)
            .NotEmpty()
            .WithMessage($"The {fieldName} is required.")
            .MinimumLength(2)
            .WithMessage($"The {fieldName} must be at least 2 characters.")
            .MaximumLength(30)
            .WithMessage($"The {fieldName} can't be more than 30 characters long.");
    }
}
