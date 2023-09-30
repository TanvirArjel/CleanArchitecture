using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed class CreateEmployeeCommand : IRequest<Guid>
{
    public CreateEmployeeCommand(
        string firstName,
        string lastName,
        Guid departmentId,
        DateTime dateOfBirth,
        string email,
        string phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        DepartmentId = departmentId;
        DateOfBirth = dateOfBirth;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public string FirstName { get; }

    public string LastName { get; }

    public Guid DepartmentId { get; }

    public DateTime DateOfBirth { get; }

    public string Email { get; }

    public string PhoneNumber { get; }
}

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
