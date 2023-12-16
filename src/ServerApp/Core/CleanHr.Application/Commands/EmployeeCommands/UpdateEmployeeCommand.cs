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
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    private readonly IEmployeeCacheHandler _employeeCacheHandler = employeeCacheHandler ?? throw new ArgumentNullException(nameof(employeeCacheHandler));
    private readonly IDepartmentRepository _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));

    public async Task Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeUpdated = await _employeeRepository.GetByIdAsync(request.Id);

        if (employeeToBeUpdated == null)
        {
            throw new EntityNotFoundException(typeof(Employee), request.Id);
        }

        employeeToBeUpdated.SetName(new EmployeeName(request.Name, request.Name));
        employeeToBeUpdated.SetDateOfBirth(new DateOfBirth(request.DateOfBirth));

        await employeeToBeUpdated.SetDepartmentAsync(_departmentRepository, request.DepartmentId);
        await employeeToBeUpdated.SetEmailAsync(_employeeRepository, new Email(request.Email));
        await employeeToBeUpdated.SetPhoneNumberAsync(_employeeRepository, new PhoneNumber(request.PhoneNumber));

        await _employeeRepository.UpdateAsync(employeeToBeUpdated);

        // Remove the cache for this employee
        await _employeeCacheHandler.RemoveDetailsByIdAsync(request.Id);
    }
}
