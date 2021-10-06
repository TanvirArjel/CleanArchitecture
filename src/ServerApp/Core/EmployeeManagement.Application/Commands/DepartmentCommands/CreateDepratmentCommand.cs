using System;
using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.DepartmentCommands
{
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
            private readonly DepartmentFactory _departmentFactory;
            private readonly IDepartmentRepository _departmentRepository;
            private readonly IDepartmentCacheHandler _departmentCacheHandler;

            public CreateDepratmentCommandHandler(
                DepartmentFactory departmentFactory,
                IDepartmentRepository departmentRepository,
                IDepartmentCacheHandler departmentCacheHandler)
            {
                _departmentFactory = departmentFactory;
                _departmentRepository = departmentRepository;
                _departmentCacheHandler = departmentCacheHandler;
            }

            public async Task<Guid> Handle(CreateDepratmentCommand request, CancellationToken cancellationToken)
            {
                request.ThrowIfNull(nameof(request));

                Department department = await _departmentFactory.CreateAsync(request.Name, request.Description);

                // Persist to the database
                await _departmentRepository.InsertAsync(department);

                // Remove the cache
                await _departmentCacheHandler.RemoveListCacheAsync();

                return department.Id;
            }
        }
    }
}
