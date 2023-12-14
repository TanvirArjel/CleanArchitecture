using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class IsRefreshTokenValidQuery(Guid userId, string refreshToken) : IRequest<bool>
{
    public Guid UserId { get; } = userId;

    public string RefreshToken { get; } = refreshToken;

    private class IsRefreshTokenValidQueryHandler(IRepository repository) : IRequestHandler<IsRefreshTokenValidQuery, bool>
    {
        public async Task<bool> Handle(IsRefreshTokenValidQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isRefreshTokenValid = await repository.ExistsAsync<RefreshToken>(rt => rt.UserId == request.UserId && rt.Token == request.RefreshToken, cancellationToken);

            return isRefreshTokenValid;
        }
    }
}
