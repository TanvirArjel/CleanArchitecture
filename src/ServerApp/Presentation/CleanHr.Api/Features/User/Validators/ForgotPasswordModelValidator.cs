using System.Data;
using System.Threading;
using CleanHr.Api.Features.User.Models;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
                                 .MustAsync(IsEmailExistentAsync)
                                 .WithMessage("The email does not belong to any user account.");
    }

    private async Task<bool> IsEmailExistentAsync(string email, CancellationToken cancellationToken)
    {
        bool isExistent = await _userManager.Users
                                            .Where(u => u.Email == email)
                                            .AnyAsync(cancellationToken);
        return isExistent;
    }
}
