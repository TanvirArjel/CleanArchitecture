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

internal class StoreUserPasswordCommandHandler : IRequestHandler<StoreUserPasswordCommand>
{
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IRepository _repository;

    public StoreUserPasswordCommandHandler(
            IPasswordHasher<ApplicationUser> passwordHasher,
            IRepository repository)
    {
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(StoreUserPasswordCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        string passwordHash = _passwordHasher.HashPassword(request.User, request.Password);

        UserOldPassword userOldPassword = new()
        {
            UserId = request.User.Id,
            PasswordHash = passwordHash,
            SetAtUtc = DateTime.UtcNow
        };

        _repository.Add(userOldPassword);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
