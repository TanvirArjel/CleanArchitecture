using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Exceptions;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

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

internal class UpdateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IEmployeeCacheHandler employeeCacheHandler,
    IDepartmentRepository departmentRepository) : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeUpdated = await employeeRepository.GetByIdAsync(request.Id);

        if (employeeToBeUpdated == null)
        {
            throw new EntityNotFoundException(typeof(Employee), request.Id);
        }

        employeeToBeUpdated.SetName(new EmployeeName(request.Name, request.Name));
        employeeToBeUpdated.SetDateOfBirth(new DateOfBirth(request.DateOfBirth));

        await employeeToBeUpdated.SetDepartmentAsync(departmentRepository, request.DepartmentId);
        await employeeToBeUpdated.SetEmailAsync(employeeRepository, new Email(request.Email));
        await employeeToBeUpdated.SetPhoneNumberAsync(employeeRepository, new PhoneNumber(request.PhoneNumber));

        await employeeRepository.UpdateAsync(employeeToBeUpdated);

        // Remove the cache for this employee
        await employeeCacheHandler.RemoveDetailsByIdAsync(request.Id);
    }
}
