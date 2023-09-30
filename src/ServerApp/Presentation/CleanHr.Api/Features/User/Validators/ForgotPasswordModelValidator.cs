using System;
using System.Data;
using CleanHr.Api.Features.User.Models;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Api.Features.User.Validators;

public sealed class ForgotPasswordModelValidator : AbstractValidator<ForgotPasswordModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ForgotPasswordModelValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(fpm => fpm.Email).NotEmpty()
                                 .WithMessage("The email is required.")
                                 .EmailAddress()
                                 .WithMessage("The email is not a valid email.")
                                 .Must(IsEmailExistent)
                                 .WithMessage("The email does not belong to any user.");
    }

    private bool IsEmailExistent(string email)
    {
        bool isExistent = _userManager.Users.Where(u => u.Email == email).Any();
        return isExistent;
    }
}
