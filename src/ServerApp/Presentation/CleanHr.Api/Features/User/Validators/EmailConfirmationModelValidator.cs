using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Api.Features.User.Validators;

public sealed class EmailConfirmationModelValidator : AbstractValidator<EmailConfirmationModel>
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
                                 .Must(IsEmailRelatedToAnyAccount)
                                 .WithMessage("The provided email is not related to any account.");

        RuleFor(ecm => ecm).Custom(ValidateCode);
    }

    private bool IsEmailRelatedToAnyAccount(string email)
    {
        bool isExistent = _userManager.Users.Where(u => u.Email == email).Any();
        return isExistent;
    }

    private void ValidateCode(EmailConfirmationModel model, ValidationContext<EmailConfirmationModel> context)
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

        EmailVerificationCode emailVerificationCode = _mediator.Send(query).GetAwaiter().GetResult();

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
