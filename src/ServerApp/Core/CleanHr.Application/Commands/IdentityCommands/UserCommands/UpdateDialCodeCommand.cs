using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateDialCodeCommand : IRequest
{
    public UpdateDialCodeCommand(Guid userId, string dialCode)
    {
        UserId = userId.ThrowIfEmpty(nameof(UserId));
        DialCode = dialCode.ThrowIfNullOrEmpty(nameof(DialCode));
    }

    public Guid UserId { get; }

    public string DialCode { get; }
}

internal class UpdateDialCodeCommandHandler : IRequestHandler<UpdateDialCodeCommand>
{
    private readonly IRepository _repository;

    public UpdateDialCodeCommandHandler(IRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task Handle(UpdateDialCodeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        ApplicationUser applicationUserToBeUpdated = await _repository.GetByIdAsync<ApplicationUser>(request.UserId, cancellationToken);

        if (applicationUserToBeUpdated == null)
        {
            throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {request.UserId}.");
        }

        string dialCode = request.DialCode.StartsWith('+') ? request.DialCode : $"+{request.DialCode}";
        applicationUserToBeUpdated.SetDialCode(dialCode);
        _repository.Update(applicationUserToBeUpdated);
        await _repository.SaveChangesAsync(cancellationToken);
    }
}
