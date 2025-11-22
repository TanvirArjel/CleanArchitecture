using FluentValidation;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class ApplicationUserValidator : AbstractValidator<ApplicationUser>
{
    public ApplicationUserValidator()
    {
        // FullName validation (composed from FirstName + LastName, each 2-30 chars)
        RuleFor(u => u.FullName)
            .NotEmpty()
            .WithMessage("The FullName is required.")
            .MinimumLength(2)
            .WithMessage("The FullName must be at least 2 characters.")
            .MaximumLength(100)
            .WithMessage("The FullName can't be more than 100 characters long.");

        // Email validation
        RuleFor(u => u.Email)
            .SetValidator(new EmailValidator());

        // UserName validation
        RuleFor(u => u.UserName)
            .NotEmpty()
            .WithMessage("The UserName is required.")
            .MinimumLength(5)
            .WithMessage("The UserName must be at least 5 characters.")
            .MaximumLength(256)
            .WithMessage("The UserName can't be more than 256 characters long.");
    }
}
