using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed record DeleteEmployeeCommand(Guid EmployeeId) : IRequest;

internal class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository) : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));

    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeDeleted = await _employeeRepository.GetByIdAsync(request.EmployeeId);

        if (employeeToBeDeleted == null)
        {
            throw new EntityNotFoundException(typeof(Employee), request.EmployeeId);
        }

        await _employeeRepository.DeleteAsync(employeeToBeDeleted);
    }
}
