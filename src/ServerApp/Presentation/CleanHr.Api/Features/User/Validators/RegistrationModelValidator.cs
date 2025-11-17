using CleanHr.Api.Features.User.Models;
using FluentValidation;

namespace CleanHr.Api.Features.User.Validators;

internal sealed class RegistrationModelValidator : AbstractValidator<RegistrationModel>
{
    public RegistrationModelValidator()
    {
        RuleFor(rm => rm.FirstName).NotEmpty()
                                   .WithMessage("The FirstName is required.")
                                   .MinimumLength(2)
                                   .MaximumLength(30)
                                   .WithMessage("The FirstName can't be more than 30 characters long.");

        RuleFor(rm => rm.LastName).NotEmpty()
                                   .WithMessage("The LastName is required.")
                                   .MinimumLength(2)
                                   .MaximumLength(30)
                                   .WithMessage("The LastName can't be more than 30 characters long.");

        RuleFor(rm => rm.Email).NotEmpty()
                                   .WithMessage("The Email is required.")
                                   .EmailAddress()
                                   .WithMessage("The Email is not a valid email.")
                                   .MaximumLength(50)
                                   .WithMessage("The Email can't be more than 30 characters long.");

        RuleFor(rm => rm.Password).NotEmpty()
                                   .WithMessage("The Password is required.")
                                   .MinimumLength(8)
                                   .WithMessage("The Password must be at least 8 characters long.")
                                   .MaximumLength(20)
                                   .WithMessage("The Password can't be more than 30 characters long.");

        RuleFor(rm => rm.ConfirmPassword).NotEmpty()
                                   .WithMessage("The ConfirmPassword is required.")
                                   .Equal(rm => rm.Password)
                                   .WithMessage("Confirm password does match with password.");
    }
}
