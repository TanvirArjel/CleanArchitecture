using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateRefreshTokenCommand(Guid userId, string token) : IRequest<Result<RefreshToken>>
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    public string Token { get; } = token.ThrowIfNullOrEmpty(nameof(token));
}

internal class UpdateRefreshTokenCommandHandler(IRepository repository) : IRequestHandler<UpdateRefreshTokenCommand, Result<RefreshToken>>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Result<RefreshToken>> Handle(UpdateRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        RefreshToken refreshTokenToBeUpdated = await _repository.GetAsync<RefreshToken>(rt => rt.UserId == request.UserId, cancellationToken);

        if (refreshTokenToBeUpdated == null)
        {
            return Result<RefreshToken>.Failure($"The RefreshToken does not exist with id value: {request.UserId}.");
        }

        Result updateResult = refreshTokenToBeUpdated.UpdateToken(request.Token);

        if (updateResult.IsSuccess == false)
        {
            return Result<RefreshToken>.Failure(updateResult.Errors);
        }

        _repository.Update(refreshTokenToBeUpdated);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result<RefreshToken>.Success(refreshTokenToBeUpdated);
    }
}
