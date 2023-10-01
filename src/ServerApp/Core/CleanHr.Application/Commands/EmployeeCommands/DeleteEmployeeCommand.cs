using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed record DeleteEmployeeCommand(Guid EmployeeId) : IRequest;

internal class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository) : IRequestHandler<DeleteEmployeeCommand>
{
    public async Task Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeDeleted = await employeeRepository.GetByIdAsync(request.EmployeeId);

        if (employeeToBeDeleted == null)
        {
            throw new EntityNotFoundException(typeof(Employee), request.EmployeeId);
        }

        await employeeRepository.DeleteAsync(employeeToBeDeleted);
    }
}
