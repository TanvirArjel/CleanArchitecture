using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class StoreUserPasswordCommand(ApplicationUser user, string password) : IRequest
{
    public ApplicationUser User { get; } = user.ThrowIfNull(nameof(user));

    public string Password { get; } = password.ThrowIfNullOrEmpty(nameof(password));
}

internal class StoreUserPasswordCommandHandler(
        IPasswordHasher<ApplicationUser> passwordHasher,
        IRepository repository) : IRequestHandler<StoreUserPasswordCommand>
{
    public async Task Handle(StoreUserPasswordCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        string passwordHash = passwordHasher.HashPassword(request.User, request.Password);

        UserOldPassword userOldPassword = new()
        {
            UserId = request.User.Id,
            PasswordHash = passwordHash,
            SetAtUtc = DateTime.UtcNow
        };

        repository.Add(userOldPassword);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
