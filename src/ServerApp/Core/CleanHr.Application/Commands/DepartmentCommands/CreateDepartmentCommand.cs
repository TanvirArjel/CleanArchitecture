using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed record CreateDepartmentCommand(string Name, string Description) : IRequest<Guid>;

internal class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDepartmentCacheHandler _departmentCacheHandler;

    public CreateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IDepartmentCacheHandler departmentCacheHandler)
    {
        ArgumentNullException.ThrowIfNull(departmentCacheHandler);
        ArgumentNullException.ThrowIfNull(departmentCacheHandler);

        _departmentRepository = departmentRepository;
        _departmentCacheHandler = departmentCacheHandler;
    }

    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

        DepartmentName departmentName = new(request.Name);

        Department department = await Department.CreateAsync(_departmentRepository, departmentName, request.Description);

        // Persist to the database
        await _departmentRepository.InsertAsync(department);

        // Remove the cache
        await _departmentCacheHandler.RemoveListAsync();

        return department.Id;
    }
}