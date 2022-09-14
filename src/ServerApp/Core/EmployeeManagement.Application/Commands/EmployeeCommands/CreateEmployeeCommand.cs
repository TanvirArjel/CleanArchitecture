using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.EmployeeCommands;

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

    private class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
    {
        private readonly EmployeeFactory _employeeFactory;
        private readonly IEmployeeRepository _employeeRepository;

        public CreateEmployeeCommandHandler(IEmployeeRepository employeeRepository, EmployeeFactory employeeFactory)
        {
            _employeeRepository = employeeRepository;
            _employeeFactory = employeeFactory;
        }

        public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            Name name = new Name(request.FirstName, request.LastName);
            DateOfBirth dateOfBirth = new DateOfBirth(request.DateOfBirth);
            Email email = new Email(request.Email);
            PhoneNumber phoneNumber = new PhoneNumber(request.PhoneNumber);

            Employee employee = _employeeFactory.Create(name, request.DepartmentId, dateOfBirth, email, phoneNumber);

            await _employeeRepository.InsertAsync(employee);
            return employee.Id;
        }
    }
}
