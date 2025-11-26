using CleanHr.Domain;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateTokenCommand(RefreshToken token) : IRequest<Result>
{
    public RefreshToken Token { get; } = token.ThrowIfNull(nameof(token));
}

internal class UpdateTokenCommandHandler(IRepository repository) : IRequestHandler<UpdateTokenCommand, Result>
{
    private readonly IRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<Result> Handle(UpdateTokenCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        _repository.Update(request.Token);
        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
