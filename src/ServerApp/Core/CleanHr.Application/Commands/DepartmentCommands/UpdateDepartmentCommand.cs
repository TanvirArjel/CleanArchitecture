using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Exceptions;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed class UpdateDepartmentCommand(
    Guid id,
    string name,
    string description,
    bool isActive) : IRequest
{
    public Guid Id { get; } = id;

    public string Name { get; } = name;

    public string Description { get; } = description;

    public bool IsActive { get; } = isActive;
}

internal class UpdateDepartmentCommandHandler(
    IDepartmentRepository departmentRepository,
    IDepartmentCacheHandler departmentCacheHandler) : IRequestHandler<UpdateDepartmentCommand>
{
    public async Task Handle(UpdateDepartmentCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Department departmentToBeUpdated = await departmentRepository.GetByIdAsync(request.Id);

        if (departmentToBeUpdated == null)
        {
            throw new EntityNotFoundException(typeof(Department), request.Id);
        }

        DepartmentName departmentName = new DepartmentName(request.Name);

        await departmentToBeUpdated.SetNameAsync(departmentRepository, departmentName);
        departmentToBeUpdated.SetDescription(request.Description);
        departmentToBeUpdated.IsActive = request.IsActive;

        await departmentRepository.UpdateAsync(departmentToBeUpdated);

        await departmentCacheHandler.RemoveListAsync();
    }
}
