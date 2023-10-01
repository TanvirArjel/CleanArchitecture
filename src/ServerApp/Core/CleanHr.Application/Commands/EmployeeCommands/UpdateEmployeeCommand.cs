using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using CleanHr.Domain.Exceptions;
using CleanHr.Domain.ValueObjects;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed record UpdateEmployeeCommand(
   Guid Id,
   string Name,
   Guid DepartmentId,
   DateTime DateOfBirth,
   string Email,
   string PhoneNumber) : IRequest;

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
