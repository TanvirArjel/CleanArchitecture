using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateRefreshTokenCommand : IRequest<RefreshToken>
{
    public UpdateRefreshTokenCommand(Guid userId, string token)
    {
        UserId = userId.ThrowIfEmpty(nameof(userId));
        Token = token.ThrowIfNullOrEmpty(nameof(token));
    }

    public Guid UserId { get; }

    public string Token { get; }

    private class UpdateRefreshTokenCommandHandler : IRequestHandler<UpdateRefreshTokenCommand, RefreshToken>
    {
        private readonly IRepository _repository;

        public UpdateRefreshTokenCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

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

            await _repository.UpdateAsync(refreshTokenToBeUpdated, cancellationToken);

            return refreshTokenToBeUpdated;
        }
    }
}
