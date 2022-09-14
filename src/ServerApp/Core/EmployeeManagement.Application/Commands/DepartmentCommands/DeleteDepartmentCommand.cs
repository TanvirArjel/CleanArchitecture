using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.DepartmentCommands;

public sealed class DeleteDepartmentCommand : IRequest
{
    public DeleteDepartmentCommand(Guid departmentId)
    {
        Id = departmentId.ThrowIfEmpty(nameof(departmentId));
    }

    public Guid Id { get; }

    private class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand>
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IDepartmentCacheHandler _departmentCacheHandler;

        public DeleteDepartmentCommandHandler(IDepartmentRepository departmentRepository, IDepartmentCacheHandler departmentCacheHandler)
        {
            _departmentRepository = departmentRepository;
            _departmentCacheHandler = departmentCacheHandler;
        }

        public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            Department department = await _departmentRepository.GetByIdAsync(request.Id);

            if (department == null)
            {
                throw new EntityNotFoundException(typeof(Department), request.Id);
            }

            await _departmentRepository.DeleteAsync(department);
            await _departmentCacheHandler.RemoveListAsync();

            return Unit.Value;
        }
    }
}
