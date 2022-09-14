using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Commands.IdentityCommands.UserCommands;

public sealed class UpdateDialCodeCommand : IRequest
{
    public UpdateDialCodeCommand(Guid userId, string dialCode)
    {
        UserId = userId.ThrowIfEmpty(nameof(UserId));
        DialCode = dialCode.ThrowIfNullOrEmpty(nameof(DialCode));
    }

    public Guid UserId { get; }

    public string DialCode { get; }

    private class UpdateDialCodeCommandHandler : IRequestHandler<UpdateDialCodeCommand>
    {
        private readonly IRepository _repository;

        public UpdateDialCodeCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(UpdateDialCodeCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            ApplicationUser applicationUserToBeUpdated = await _repository.GetByIdAsync<ApplicationUser>(request.UserId, cancellationToken);

            if (applicationUserToBeUpdated == null)
            {
                throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {request.UserId}.");
            }

            applicationUserToBeUpdated.DialCode = request.DialCode.StartsWith('+') ? request.DialCode : $"+{request.DialCode}";
            await _repository.UpdateAsync(applicationUserToBeUpdated, cancellationToken);

            return Unit.Value;
        }
    }
}
