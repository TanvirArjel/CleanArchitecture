using FluentValidation;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class EmailValidator : AbstractValidator<string>
{
    public EmailValidator()
    {
        RuleFor(email => email)
            .NotEmpty()
            .WithMessage("The Email is required.")
            .EmailAddress()
            .WithMessage("The Email is not a valid email.")
            .MaximumLength(50)
            .WithMessage("The Email can't be more than 50 characters long.");
    }
}
