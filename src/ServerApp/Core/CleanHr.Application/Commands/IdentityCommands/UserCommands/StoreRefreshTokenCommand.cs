using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class StoreRefreshTokenCommand(Guid userId, string token) : IRequest<Result<RefreshToken>>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    public string Token { get; } = token.ThrowIfNullOrEmpty(nameof(token));
}

internal class StoreRefreshTokenCommandHandler : IRequestHandler<StoreRefreshTokenCommand, Result<RefreshToken>>
{
    private readonly IRepository _repository;

    public StoreRefreshTokenCommandHandler(IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Result<RefreshToken>> Handle(StoreRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Result<RefreshToken> result = await RefreshToken.CreateAsync(request.UserId, request.Token);

        if (result.IsSuccess == false)
        {
            return result;
        }

        RefreshToken refreshToken = result.Value;

        _repository.Add(refreshToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<RefreshToken>.Success(refreshToken);
    }
}
