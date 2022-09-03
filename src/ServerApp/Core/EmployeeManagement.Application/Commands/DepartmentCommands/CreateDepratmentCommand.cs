using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Aggregates.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.DepartmentCommands;

public class CreateDepratmentCommand : IRequest<Guid>
{
    public CreateDepratmentCommand(string name, string description)
    {
        Name = name;
        Description = description;
    }

    public string Name { get; }

    public string Description { get; }

    private class CreateDepratmentCommandHandler : IRequestHandler<CreateDepratmentCommand, Guid>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentCacheHandler _departmentCacheHandler;

        public CreateDepratmentCommandHandler(
            IDepartmentRepository departmentRepository,
            IDepartmentCacheHandler departmentCacheHandler)
        {
            _departmentRepository = departmentRepository;
            _departmentCacheHandler = departmentCacheHandler;
        }

        public async Task<Guid> Handle(CreateDepratmentCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            DepartmentName departmentName = new DepartmentName(request.Name);

            Department department = await Department.CreateAsync(_departmentRepository, departmentName, request.Description);

            // Persist to the database
            await _departmentRepository.InsertAsync(department);

            // Remove the cache
            await _departmentCacheHandler.RemoveListAsync();

            return department.Id;
        }
    }
}
