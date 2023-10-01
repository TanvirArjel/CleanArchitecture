using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateLanguageCultureCommand(Guid userId, string languageCulture) : IRequest
{
    public Guid UserId { get; } = userId.ThrowIfEmpty(nameof(userId));

    public string LanguageCulture { get; } = languageCulture.ThrowIfNullOrEmpty(nameof(languageCulture));
}

internal class UpdateLanguageCultureCommandHandler(IRepository repository) : IRequestHandler<UpdateLanguageCultureCommand>
{
    public async Task Handle(UpdateLanguageCultureCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        ApplicationUser userToBeUpdated = await repository.GetByIdAsync<ApplicationUser>(request.UserId, cancellationToken);

        if (userToBeUpdated == null)
        {
            throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {request.UserId}.");
        }

        userToBeUpdated.LanguageCulture = request.LanguageCulture;

        repository.Update(userToBeUpdated);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
