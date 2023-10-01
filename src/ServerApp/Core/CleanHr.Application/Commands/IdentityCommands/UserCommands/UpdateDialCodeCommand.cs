﻿using CleanHr.Domain.Aggregates.IdentityAggregate;
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

internal class UpdateDialCodeCommandHandler(IRepository repository) : IRequestHandler<UpdateDialCodeCommand>
{
    public async Task Handle(UpdateDialCodeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        ApplicationUser applicationUserToBeUpdated = await repository.GetByIdAsync<ApplicationUser>(request.UserId, cancellationToken);

        if (applicationUserToBeUpdated == null)
        {
            throw new InvalidOperationException($"The ApplicationUser does not exist with id value: {request.UserId}.");
        }

        applicationUserToBeUpdated.DialCode = request.DialCode.StartsWith('+') ? request.DialCode : $"+{request.DialCode}";
        repository.Update(applicationUserToBeUpdated);
        await repository.SaveChangesAsync(cancellationToken);
    }
}
