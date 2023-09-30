using System;
using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Api.Features.User.Validators;

public sealed class ResendEmailConfirmationCodeModelValidator : AbstractValidator<ResendEmailConfirmationCodeModel>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public ResendEmailConfirmationCodeModelValidator(
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;

        RuleFor(ecc => ecc.Email).NotEmpty()
                                .WithMessage("The email is required.")
                                .EmailAddress()
                                .WithMessage("The email is not a valid email.")
                                .Custom(ValidateEmail);
        _mediator = mediator;
    }

    private void ValidateEmail(string email, ValidationContext<ResendEmailConfirmationCodeModel> context)
    {
        ApplicationUser applicationUser = _userManager.FindByEmailAsync(email).GetAwaiter().GetResult();

        if (applicationUser == null)
        {
            context.AddFailure("Email", "The provided email is not related to any account.");
            return;
        }

        if (applicationUser.EmailConfirmed)
        {
            context.AddFailure("email", "The email is already confirmed.");
            return;
        }

        HasUserActiveEmailVerificationCodeQuery query = new HasUserActiveEmailVerificationCodeQuery(email);

        bool isExists = _mediator.Send(query).GetAwaiter().GetResult();

        if (isExists)
        {
            context.AddFailure("email", "You already have an active code. Please wait! You may receive the code in your email. If not, please try again after sometimes.");
        }
    }
}
