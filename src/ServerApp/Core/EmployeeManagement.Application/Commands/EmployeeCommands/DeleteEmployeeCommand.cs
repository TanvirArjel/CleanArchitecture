using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.EmployeeCommands;

public class DeleteEmployeeCommand : IRequest
{
    public DeleteEmployeeCommand(Guid employeeId)
    {
        EmployeeId = employeeId;
    }

    public Guid EmployeeId { get; }

    private class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            Employee employeeToBeDeleted = await _employeeRepository.GetByIdAsync(request.EmployeeId);

            if (employeeToBeDeleted == null)
            {
                throw new EntityNotFoundException(typeof(Employee), request.EmployeeId);
            }

            await _employeeRepository.DeleteAsync(employeeToBeDeleted);

            return Unit.Value;
        }
    }
}
