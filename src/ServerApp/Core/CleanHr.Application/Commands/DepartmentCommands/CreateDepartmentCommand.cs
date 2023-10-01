using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed record CreateDepartmentCommand(string Name, string Description) : IRequest<Guid>;

internal class CreateDepartmentCommandHandler(
        IDepartmentRepository departmentRepository,
        IDepartmentCacheHandler departmentCacheHandler) : IRequestHandler<CreateDepartmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

        DepartmentName departmentName = new(request.Name);

        Department department = await Department.CreateAsync(departmentRepository, departmentName, request.Description);

        // Persist to the database
        await departmentRepository.InsertAsync(department);

        // Remove the cache
        await departmentCacheHandler.RemoveListAsync();

        return department.Id;
    }
}