using CleanHr.Domain;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed record DeleteEmployeeCommand(Guid EmployeeId) : IRequest<Result>;

internal class DeleteEmployeeCommandHandler(
    IEmployeeRepository employeeRepository) : IRequestHandler<DeleteEmployeeCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));

    public async Task<Result> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeDeleted = await _employeeRepository.GetByIdAsync(request.EmployeeId);

        if (employeeToBeDeleted == null)
        {
            return Result.Failure($"The Employee does not exist with id value: {request.EmployeeId}.");
        }

        await _employeeRepository.DeleteAsync(employeeToBeDeleted);

        return Result.Success();
    }
}
