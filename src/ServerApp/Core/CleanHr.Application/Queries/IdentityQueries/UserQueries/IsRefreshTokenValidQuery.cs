using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class IsRefreshTokenValidQuery : IRequest<bool>
{
    public IsRefreshTokenValidQuery(Guid userId, string refreshToken)
    {
        UserId = userId;
        RefreshToken = refreshToken;
    }

    public Guid UserId { get; }

    public string RefreshToken { get; }

    private class IsRefreshTokenValidQueryHandler : IRequestHandler<IsRefreshTokenValidQuery, bool>
    {
        private readonly IRepository _repository;

        public IsRefreshTokenValidQueryHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(IsRefreshTokenValidQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isRefreshTokenValid = await _repository.ExistsAsync<RefreshToken>(rt => rt.UserId == request.UserId && rt.Token == request.RefreshToken, cancellationToken);

            return isRefreshTokenValid;
        }
    }
}
