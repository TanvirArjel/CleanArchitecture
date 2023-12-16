using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateRefreshTokenCommand(Guid userId, string token) : IRequest<RefreshToken>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    public string Token { get; } = token.ThrowIfNullOrEmpty(nameof(token));
}

internal class UpdateRefreshTokenCommandHandler(IRepository repository) : IRequestHandler<UpdateRefreshTokenCommand, RefreshToken>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<RefreshToken> Handle(UpdateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        RefreshToken refreshTokenToBeUpdated = await _repository.GetAsync<RefreshToken>(rt => rt.UserId == request.UserId, cancellationToken);

        if (refreshTokenToBeUpdated == null)
        {
            throw new InvalidOperationException($"The RefreshToken does not exist with id value: {request.UserId}.");
        }

        refreshTokenToBeUpdated.Token = request.Token;
        refreshTokenToBeUpdated.ExpireAtUtc = DateTime.UtcNow;
        refreshTokenToBeUpdated.CreatedAtUtc = DateTime.UtcNow.AddDays(10);

        _repository.Update(refreshTokenToBeUpdated);
        await _repository.SaveChangesAsync(cancellationToken);

        return refreshTokenToBeUpdated;
    }
}
