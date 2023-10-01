using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    Guid DepartmentId,
    DateTime DateOfBirth,
    string Email,
    string PhoneNumber) : IRequest<Guid>;

internal class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    EmployeeFactory employeeFactory) : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        EmployeeName name = new(request.FirstName, request.LastName);
        DateOfBirth dateOfBirth = new(request.DateOfBirth);
        Email email = new(request.Email);
        PhoneNumber phoneNumber = new(request.PhoneNumber);

        Employee employee = employeeFactory.Create(name, request.DepartmentId, dateOfBirth, email, phoneNumber);

        await employeeRepository.InsertAsync(employee);
        return employee.Id;
    }
}
