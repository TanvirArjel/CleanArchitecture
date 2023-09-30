using System;
using CleanHr.Api.Features.User.Models;
using FluentValidation;

namespace CleanHr.Api.Features.User.Validators;

public sealed class LoginModelValidator : AbstractValidator<LoginModel>
{
    public LoginModelValidator()
    {
        RuleFor(lm => lm.EmailOrUserName).NotEmpty()
                                         .WithMessage("The emailOrUserName is required.")
                                         .MinimumLength(5)
                                         .MaximumLength(50)
                                         .Must(IsEmailOrUserNameExistent)
                                         .WithMessage("The emailOrUserName does not exist.");

        RuleFor(lm => lm.Password).NotEmpty()
                                        .WithMessage("The Password is required.")
                                        .MinimumLength(6)
                                        .MaximumLength(50);
    }

    private bool IsEmailOrUserNameExistent(string emailOrUserName)
    {
        return true;
    }
}
