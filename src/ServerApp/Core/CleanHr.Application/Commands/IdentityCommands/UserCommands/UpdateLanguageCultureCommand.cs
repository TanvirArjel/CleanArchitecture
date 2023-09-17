using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateLanguageCultureCommand : IRequest
{
    public UpdateLanguageCultureCommand(Guid userId, string languageCulture)
    {
        UserId = userId.ThrowIfEmpty(nameof(userId));
        LanguageCulture = languageCulture.ThrowIfNullOrEmpty(nameof(languageCulture));
    }

    public Guid UserId { get; }

    public string LanguageCulture { get; }

    private class UpdateLanguageCultureCommandHandler : IRequestHandler<UpdateLanguageCultureCommand>
    {
        private readonly IRepository _repository;

        public UpdateLanguageCultureCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(UpdateLanguageCultureCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            ApplicationUser userToBeUpdated = await _repository.GetByIdAsync<ApplicationUser>(request.UserId, cancellationToken);

            if (userToBeUpdated == null)
            {
                throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {request.UserId}.");
            }

            userToBeUpdated.LanguageCulture = request.LanguageCulture;

            _repository.Update(userToBeUpdated);
            await _repository.SaveChangesAsync(cancellationToken);
        }
    }
}
