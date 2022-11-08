using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.EmployeeCommands;

public sealed class UpdateEmployeeCommand : IRequest
{
    public UpdateEmployeeCommand(
       Guid id,
       string name,
       Guid departmentId,
       DateTime dateOfBirth,
       string email,
       string phoneNumber)
    {
        Id = id;
        Name = name;
        DepartmentId = departmentId;
        DateOfBirth = dateOfBirth;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Guid Id { get; }

    public string Name { get; }

    public Guid DepartmentId { get; }

    public DateTime DateOfBirth { get; }

    public string Email { get; }

    public string PhoneNumber { get; }
}

internal class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeCacheHandler _employeeCacheHandler;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IEmployeeCacheHandler employeeCacheHandler,
        IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _employeeCacheHandler = employeeCacheHandler;
        _departmentRepository = departmentRepository;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeUpdated = await _employeeRepository.GetByIdAsync(request.Id);

        if (employeeToBeUpdated == null)
        {
            throw new EntityNotFoundException(typeof(Employee), request.Id);
        }

        employeeToBeUpdated.SetName(new Name(request.Name, request.Name));
        employeeToBeUpdated.SetDateOfBirth(new DateOfBirth(request.DateOfBirth));

        await employeeToBeUpdated.SetDepartmentAsync(_departmentRepository, request.DepartmentId);
        await employeeToBeUpdated.SetEmailAsync(_employeeRepository, new Email(request.Email));
        await employeeToBeUpdated.SetPhoneNumberAsync(_employeeRepository, new PhoneNumber(request.PhoneNumber));

        await _employeeRepository.UpdateAsync(employeeToBeUpdated);

        // Remove the cache for this employee
        await _employeeCacheHandler.RemoveDetailsByIdAsync(request.Id);

        return Unit.Value;
    }
}
