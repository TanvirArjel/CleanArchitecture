using System.Threading;
using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanHr.Api.Features.User.Validators;

internal sealed class EmailConfirmationModelValidator : AbstractValidator<EmailConfirmationModel>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public EmailConfirmationModelValidator(
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;

        RuleFor(ecm => ecm.Email).NotEmpty()
                                 .WithMessage("The email is required.")
                                 .EmailAddress()
                                 .WithMessage("The email is not a valid email.")
                                 .MustAsync(IsEmailRelatedToAnyAccountAsync)
                                 .WithMessage("The provided email is not related to any account.")
                                 .DependentRules(() =>
                                 {
                                     RuleFor(ecm => ecm).CustomAsync(ValidateCodeAsync);
                                 });
    }

    private async Task<bool> IsEmailRelatedToAnyAccountAsync(string email, CancellationToken cancellationToken)
    {
        bool isExistent = await _userManager.Users
                                            .Where(u => u.Email == email)
                                            .AnyAsync(cancellationToken);
        return isExistent;
    }

    private async Task ValidateCodeAsync(
        EmailConfirmationModel model,
        ValidationContext<EmailConfirmationModel> context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(model.Code))
        {
            context.AddFailure(nameof(model.Code), "The code is required.");
            return;
        }

        if (model.Code.Length != 6)
        {
            context.AddFailure(nameof(model.Code), "The code must be 6 characters long.");
            return;
        }

        GetEmailVerificationCodeQuery query = new(model.Email, model.Code);

        EmailVerificationCode emailVerificationCode = await _mediator.Send(query, cancellationToken);

        if (emailVerificationCode == null)
        {
            context.AddFailure(nameof(model.Code), "The code is not valid.");
            return;
        }

        if (DateTime.UtcNow > emailVerificationCode.SentAtUtc.AddMinutes(5))
        {
            context.AddFailure(nameof(model.Code), "The code is expired.");
        }
    }
}
