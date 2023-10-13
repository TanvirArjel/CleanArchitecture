using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class StoreRefreshTokenCommand(Guid userId, string token) : IRequest<RefreshToken>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    public string Token { get; } = token.ThrowIfNullOrEmpty(nameof(token));
}

internal class StoreRefreshTokenCommandHandler(IRepository repository) : IRequestHandler<StoreRefreshTokenCommand, RefreshToken>
{
    public async Task<RefreshToken> Handle(StoreRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        RefreshToken refreshToken = new()
        {
            UserId = request.UserId,
            Token = request.Token,
            CreatedAtUtc = DateTime.UtcNow,
            ExpireAtUtc = DateTime.UtcNow.AddDays(30)
        };

        repository.Add(refreshToken);
        await repository.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }
}
