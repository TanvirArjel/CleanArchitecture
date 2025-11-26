using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.IdentityQueries.UserQueries;

public sealed class IsRefreshTokenValidQuery(string refreshToken) : IRequest<bool>
{
    public string RefreshToken { get; } = refreshToken.ThrowIfNullOrEmpty(nameof(refreshToken));
}

internal class IsRefreshTokenValidQueryHandler(IRepository repository) : IRequestHandler<IsRefreshTokenValidQuery, bool>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<bool> Handle(IsRefreshTokenValidQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        RefreshToken refreshToken = await _repository.GetAsync<RefreshToken>(
            rt => rt.Token == request.RefreshToken,
            cancellationToken);

        if (refreshToken == null)
        {
            return false;
        }

        // Check if the refresh token is valid
        return refreshToken.IsValid();
    }
}
