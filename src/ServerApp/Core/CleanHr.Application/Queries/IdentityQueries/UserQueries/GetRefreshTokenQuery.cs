using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class GetRefreshTokenQuery(string token) : IRequest<RefreshToken>
{
    public string Token { get; } = token.ThrowIfNullOrEmpty(nameof(token));

    private class GetRefreshTokenQueryHanlder(IRepository repository) : IRequestHandler<GetRefreshTokenQuery, RefreshToken>
    {
        public async Task<RefreshToken> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            RefreshToken refreshToken = await repository.GetAsync<RefreshToken>(
                rt => rt.Token == request.Token && !rt.IsRevoked,
                cancellationToken);

            return refreshToken;
        }
    }
}
