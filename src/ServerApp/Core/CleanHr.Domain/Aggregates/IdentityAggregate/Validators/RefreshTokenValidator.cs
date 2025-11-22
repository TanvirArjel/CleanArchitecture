using System;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class RefreshTokenValidator : AbstractValidator<RefreshToken>
{
    public RefreshTokenValidator()
    {
        RuleFor(rt => rt.UserId)
            .NotEmpty()
            .WithMessage("The UserId is required.");

        RuleFor(rt => rt.Token)
            .NotEmpty()
            .WithMessage("The Token is required.")
            .MinimumLength(10)
            .WithMessage("The Token must be at least 10 characters.");

        RuleFor(rt => rt.CreatedAtUtc)
            .NotEmpty()
            .WithMessage("The CreatedAtUtc is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The CreatedAtUtc cannot be a future date.");

        RuleFor(rt => rt.ExpireAtUtc)
            .NotEmpty()
            .WithMessage("The ExpireAtUtc is required.")
            .GreaterThan(rt => rt.CreatedAtUtc)
            .WithMessage("The ExpireAtUtc must be after CreatedAtUtc.");
    }
}
