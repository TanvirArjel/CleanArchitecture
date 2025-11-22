using System;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class PasswordResetCodeValidator : AbstractValidator<PasswordResetCode>
{
    public PasswordResetCodeValidator()
    {
        RuleFor(p => p.Email)
            .SetValidator(new EmailValidator());

        RuleFor(p => p.Code)
            .NotEmpty()
            .WithMessage("The Code is required.")
            .Length(6)
            .WithMessage("The Code must be exactly 6 characters long.")
            .Matches("^[0-9]{6}$")
            .WithMessage("The Code must contain only digits.");

        RuleFor(p => p.SentAtUtc)
            .NotEmpty()
            .WithMessage("The SentAtUtc is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The SentAtUtc cannot be in the future.");
    }
}
