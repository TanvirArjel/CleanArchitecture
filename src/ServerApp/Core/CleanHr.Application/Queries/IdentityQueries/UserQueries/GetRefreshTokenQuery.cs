using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class GetRefreshTokenQuery(Guid userId) : IRequest<RefreshToken>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    private class GetRefreshTokenQueryHanlder(IRepository repository) : IRequestHandler<GetRefreshTokenQuery, RefreshToken>
    {
        public async Task<RefreshToken> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            RefreshToken refreshToken = await repository.GetAsync<RefreshToken>(rt => rt.UserId == request.UserId, cancellationToken);

            return refreshToken;
        }
    }
}
