using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class RotateRefreshTokenCommand(Guid userId, string newToken, string tokenFamily, Guid? previousTokenId) : IRequest<Result<RefreshToken>>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    public string NewToken { get; } = newToken.ThrowIfNullOrEmpty(nameof(newToken));

    public string TokenFamily { get; } = tokenFamily.ThrowIfNullOrEmpty(nameof(tokenFamily));

    public Guid? PreviousTokenId { get; } = previousTokenId;
}

internal class RotateRefreshTokenCommandHandler(IRepository repository) : IRequestHandler<RotateRefreshTokenCommand, Result<RefreshToken>>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Result<RefreshToken>> Handle(RotateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        // Create new token in the same family chain
        Result<RefreshToken> result = await RefreshToken.CreateAsync(
            request.UserId,
            request.NewToken,
            tokenFamily: request.TokenFamily,
            previousTokenId: request.PreviousTokenId);

        if (result.IsSuccess == false)
        {
            return result;
        }

        RefreshToken newRefreshToken = result.Value;

        _repository.Add(newRefreshToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<RefreshToken>.Success(newRefreshToken);
    }
}
