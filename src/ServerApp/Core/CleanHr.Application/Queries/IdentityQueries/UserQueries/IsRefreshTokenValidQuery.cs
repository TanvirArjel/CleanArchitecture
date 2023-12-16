using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class IsRefreshTokenValidQuery(Guid userId, string refreshToken) : IRequest<bool>
{
    public Guid UserId { get; } = userId;

    public string RefreshToken { get; } = refreshToken;
}

internal class IsRefreshTokenValidQueryHandler(IRepository repository) : IRequestHandler<IsRefreshTokenValidQuery, bool>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<bool> Handle(IsRefreshTokenValidQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        bool isRefreshTokenValid = await _repository.ExistsAsync<RefreshToken>(rt => rt.UserId == request.UserId && rt.Token == request.RefreshToken, cancellationToken);

        return isRefreshTokenValid;
    }
}
