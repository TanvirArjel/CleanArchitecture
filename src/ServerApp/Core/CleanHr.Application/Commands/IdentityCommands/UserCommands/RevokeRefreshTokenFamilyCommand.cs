using System.Linq;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class RevokeRefreshTokenFamilyCommand(string tokenFamily) : IRequest<Result>
{
    public string TokenFamily { get; } = tokenFamily.ThrowIfNullOrEmpty(nameof(tokenFamily));
}

internal class RevokeRefreshTokenFamilyCommandHandler(IRepository repository) : IRequestHandler<RevokeRefreshTokenFamilyCommand, Result>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Result> Handle(RevokeRefreshTokenFamilyCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        // Get all tokens in the family
        List<RefreshToken> tokensInFamily = await _repository
            .GetQueryable<RefreshToken>()
            .Where(rt => rt.TokenFamily == request.TokenFamily && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        if (tokensInFamily.Count == 0)
        {
            return Result.Success(); // Already revoked or doesn't exist
        }

        // Revoke all tokens in the family
        foreach (RefreshToken token in tokensInFamily)
        {
            token.Revoke();
            _repository.Update(token);
        }

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
