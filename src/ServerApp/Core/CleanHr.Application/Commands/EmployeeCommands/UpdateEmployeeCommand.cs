using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public sealed record UpdateEmployeeCommand(
   Guid Id,
   string FirstName,
   string LastName,
   Guid DepartmentId,
   DateTime DateOfBirth,
   string Email,
   string PhoneNumber) : IRequest<Result>;

internal class UpdateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IEmployeeCacheHandler employeeCacheHandler,
    IDepartmentRepository departmentRepository) : IRequestHandler<UpdateEmployeeCommand, Result>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    private readonly IEmployeeCacheHandler _employeeCacheHandler = employeeCacheHandler ?? throw new ArgumentNullException(nameof(employeeCacheHandler));
    private readonly IDepartmentRepository _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));

    public async Task<Result> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Employee employeeToBeUpdated = await _employeeRepository.GetByIdAsync(request.Id);

        if (employeeToBeUpdated == null)
        {
            return Result.Failure("EmployeeId", $"The employee with id '{request.Id}' was not found.");
        }

        Result nameResult = employeeToBeUpdated.SetName(request.FirstName, request.LastName);
        if (nameResult.IsSuccess == false)
        {
            return nameResult;
        }

        Result dateOfBirthResult = employeeToBeUpdated.SetDateOfBirth(request.DateOfBirth);
        if (dateOfBirthResult.IsSuccess == false)
        {
            return dateOfBirthResult;
        }

        Result departmentResult = await employeeToBeUpdated.SetDepartmentAsync(_departmentRepository, request.DepartmentId);
        if (departmentResult.IsSuccess == false)
        {
            return departmentResult;
        }

        Result emailResult = await employeeToBeUpdated.SetEmailAsync(_employeeRepository, request.Email);
        if (emailResult.IsSuccess == false)
        {
            return emailResult;
        }

        Result phoneNumberResult = await employeeToBeUpdated.SetPhoneNumberAsync(_employeeRepository, request.PhoneNumber);
        if (phoneNumberResult.IsSuccess == false)
        {
            return phoneNumberResult;
        }

        await _employeeRepository.UpdateAsync(employeeToBeUpdated);

        // Remove the cache for this employee
        await _employeeCacheHandler.RemoveDetailsByIdAsync(request.Id);

        return Result.Success();
    }
}
