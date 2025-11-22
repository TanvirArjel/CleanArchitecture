using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class EmailVerificationCodeValidator : AbstractValidator<EmailVerificationCode>
{
    private readonly UserManager<ApplicationUser> _userManager;
    public EmailVerificationCodeValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(e => e.Email)
            .SetValidator(new EmailValidator())
            .CustomAsync(ValidateEmailAsync);

        RuleFor(e => e.Code)
            .NotEmpty()
            .WithMessage("The Code is required.")
            .Length(6)
            .WithMessage("The Code must be exactly 6 characters long.")
            .Matches("^[0-9]{6}$")
            .WithMessage("The Code must contain only digits.");

        RuleFor(e => e.SentAtUtc)
            .NotEmpty()
            .WithMessage("The SentAtUtc is required.")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("The SentAtUtc cannot be in the future.");
    }

    private async Task ValidateEmailAsync(
        string email,
        ValidationContext<EmailVerificationCode> context,
        CancellationToken cancellationToken)
    {
        ApplicationUser applicationUser = await _userManager.FindByEmailAsync(email);

        if (applicationUser == null)
        {
            context.AddFailure("Email", "The provided email is not related to any account.");
            return;
        }

        if (applicationUser.EmailConfirmed)
        {
            context.AddFailure("Email", "The email is already confirmed.");
            return;
        }

        // HasUserActiveEmailVerificationCodeQuery query = new(email);

        // bool isExists = await _mediator.Send(query, cancellationToken);

        // if (isExists)
        // {
        //     context.AddFailure("email", "You already have an active code. Please wait! You may receive the code in your email. If not, please try again after sometimes.");
        // }
    }
}
