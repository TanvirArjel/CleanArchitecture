using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed class DeleteEmployeeCommand : IRequest
{
    public DeleteEmployeeCommand(Guid employeeId)
    {
        EmployeeId = employeeId;
    }

    public Guid EmployeeId { get; }
}

internal class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

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
