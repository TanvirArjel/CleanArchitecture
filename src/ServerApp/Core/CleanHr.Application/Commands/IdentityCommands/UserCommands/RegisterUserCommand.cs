using System.Linq;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<Result<Guid>>;

internal class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<Guid>>
{
    private readonly ApplicationUserFactory _userFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public RegisterUserCommandHandler(
        ApplicationUserFactory userFactory,
        UserManager<ApplicationUser> userManager)
    {
        _userFactory = userFactory ?? throw new ArgumentNullException(nameof(userFactory));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        // Create user through domain factory (with password validation)
        Result<ApplicationUser> result = await _userFactory.CreateAsync(
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password);

        if (result.IsSuccess == false)
        {
            return Result<Guid>.Failure(result.Errors);
        }

        // Use UserManager to handle password hashing and persistence
        IdentityResult identityResult = await _userManager.CreateAsync(result.Value, request.Password);

        if (identityResult.Succeeded == false)
        {
            Dictionary<string, string> errors = identityResult.Errors.ToDictionary(
                e => e.Code,
                e => e.Description);
            return Result<Guid>.Failure(errors);
        }

        return Result<Guid>.Success(result.Value.Id);
    }
}
