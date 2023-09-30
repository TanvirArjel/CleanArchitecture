using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed class DeleteDepartmentCommand : IRequest
{
    public DeleteDepartmentCommand(Guid departmentId)
    {
        Id = departmentId.ThrowIfEmpty(nameof(departmentId));
    }

    public Guid Id { get; }
}

internal class DeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository, IDepartmentCacheHandler departmentCacheHandler) : IRequestHandler<DeleteDepartmentCommand>
{
    public async Task Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

        Department department = await departmentRepository.GetByIdAsync(request.Id);

        if (department == null)
        {
            throw new EntityNotFoundException(typeof(Department), request.Id);
        }

        await departmentRepository.DeleteAsync(department);
        await departmentCacheHandler.RemoveListAsync();
    }
}