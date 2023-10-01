using System.Threading;
using CleanHr.Api.Features.User.Models;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanHr.Api.Features.User.Validators;

public sealed class LoginModelValidator : AbstractValidator<LoginModel>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModelValidator(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;

        RuleFor(lm => lm.EmailOrUserName).NotEmpty()
                                         .WithMessage("The emailOrUserName is required.")
                                         .MinimumLength(5)
                                         .MaximumLength(50)
                                         .MustAsync(IsEmailOrUserNameExistentAsync)
                                         .WithMessage("The emailOrUserName does not exist.");

        RuleFor(lm => lm.Password).NotEmpty()
                                        .WithMessage("The Password is required.")
                                        .MinimumLength(6)
                                        .MaximumLength(50);
    }

    private async Task<bool> IsEmailOrUserNameExistentAsync(
        string emailOrUserName,
        CancellationToken cancellationToken)
    {
        string emailUpper = emailOrUserName.ToUpperInvariant();
        var isExistent = await _userManager.Users
                          .Where(u => u.NormalizedEmail == emailUpper || u.NormalizedUserName == emailUpper)
                          .AnyAsync(cancellationToken);
        return isExistent;
    }
}
