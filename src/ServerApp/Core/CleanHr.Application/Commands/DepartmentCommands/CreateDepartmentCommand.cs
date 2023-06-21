using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.DepartmentCommands;

public sealed class CreateDepartmentCommand : IRequest<Guid>
{
    public CreateDepartmentCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string Description { get; }
}

public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Guid>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDepartmentCacheHandler _departmentCacheHandler;

    public CreateDepartmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IDepartmentCacheHandler departmentCacheHandler)
    {
        _departmentRepository = departmentRepository;
        _departmentCacheHandler = departmentCacheHandler;
    }

    public async Task<Guid> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
    {
        _ = request.ThrowIfNull(nameof(request));

        DepartmentName departmentName = new DepartmentName(request.Name);

        Department department = await Department.CreateAsync(_departmentRepository, departmentName, request.Description);

        // Persist to the database
        await _departmentRepository.InsertAsync(department);

        // Remove the cache
        await _departmentCacheHandler.RemoveListAsync();

        return department.Id;
    }
}