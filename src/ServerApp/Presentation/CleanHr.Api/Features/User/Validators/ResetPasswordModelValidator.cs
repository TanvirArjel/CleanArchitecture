using System.Threading;
using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using MediatR;

namespace CleanHr.Api.Features.User.Validators;

public sealed class ResetPasswordModelValidator : AbstractValidator<ResetPasswordModel>
{
    private readonly IMediator _mediator;

    public ResetPasswordModelValidator(IMediator mediator)
    {
        _mediator = mediator;

        RuleFor(rpm => rpm.Email).NotEmpty()
                                 .WithMessage("The email is required.")
                                 .EmailAddress()
                                 .WithMessage("The email is not a valid email.")
                                 .MaximumLength(50)
                                 .WithMessage("The email cannot be more than 50 characters.");

        RuleFor(rpm => rpm.Password).NotEmpty()
                                 .WithMessage("The password is required.")
                                 .MinimumLength(8)
                                 .WithMessage("The password must be at least 8 characters long.")
                                 .MaximumLength(50)
                                 .WithMessage("The password cannot be more than 50 characters.");

        RuleFor(rpm => rpm.ConfirmPassword).NotEmpty()
                                 .WithMessage("The confirmPassword is required.")
                                 .Equal(rpm => rpm.ConfirmPassword)
                                 .WithMessage("The confirmPassword does not match with password.");

        RuleFor(rpm => rpm.Code).NotEmpty()
                                 .WithMessage("The code is required.")
                                 .Length(6)
                                 .WithMessage("The code should be exactly 6 characters long.");

        RuleFor(rpm => rpm).CustomAsync(ValidateCodeAsync);
    }

    private async Task ValidateCodeAsync(
        ResetPasswordModel model,
        ValidationContext<ResetPasswordModel> context,
        CancellationToken cancellationToken)
    {
        GetPasswordResetCodeQuery query = new(model.Email, model.Code);
        PasswordResetCode passwordResetCode = await _mediator.Send(query, cancellationToken);

        if (passwordResetCode == null)
        {
            context.AddFailure(string.Empty, "Either email or password reset code is incorrect");
            return;
        }

        if (DateTime.UtcNow > passwordResetCode.SentAtUtc.AddMinutes(5))
        {
            context.AddFailure(nameof(model.Code), "The code is expired.");
        }
    }
}
