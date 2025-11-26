using System.Linq;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class RevokeAllUserRefreshTokensCommand(Guid userId) : IRequest<Result>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));
}

internal class RevokeAllUserRefreshTokensCommandHandler(IRepository repository) : IRequestHandler<RevokeAllUserRefreshTokensCommand, Result>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Result> Handle(RevokeAllUserRefreshTokensCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        // Get all active (non-revoked) tokens for the user
        List<RefreshToken> userTokens = await _repository
            .GetQueryable<RefreshToken>()
            .Where(rt => rt.UserId == request.UserId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        if (userTokens.Count == 0)
        {
            return Result.Success(); // No tokens to revoke
        }

        // Revoke all tokens
        foreach (RefreshToken token in userTokens)
        {
            token.Revoke();
            _repository.Update(token);
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
