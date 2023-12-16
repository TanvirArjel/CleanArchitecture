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
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    private readonly EmployeeFactory _employeeFactory = employeeFactory ?? throw new ArgumentNullException(nameof(employeeFactory));

    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        EmployeeName name = new(request.FirstName, request.LastName);
        DateOfBirth dateOfBirth = new(request.DateOfBirth);
        Email email = new(request.Email);
        PhoneNumber phoneNumber = new(request.PhoneNumber);

        Employee employee = _employeeFactory.Create(name, request.DepartmentId, dateOfBirth, email, phoneNumber);

        await _employeeRepository.InsertAsync(employee);
        return employee.Id;
    }
}
