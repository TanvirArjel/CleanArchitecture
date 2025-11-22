using FluentValidation;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .NotEmpty()
            .WithMessage("The Password is required.")
            .MinimumLength(8)
            .WithMessage("The Password must be at least 8 characters long.")
            .MaximumLength(20)
            .WithMessage("The Password cannot be more than 20 characters.");
    }
}
