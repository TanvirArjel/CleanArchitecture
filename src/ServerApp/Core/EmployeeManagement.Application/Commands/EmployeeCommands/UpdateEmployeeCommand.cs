using EmployeeManagement.Application.Caching.Handlers;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using EmployeeManagement.Domain.Exceptions;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Commands.EmployeeCommands;

public class UpdateEmployeeCommand : IRequest
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

    private class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly EmployeeManipulator _employeeManipulator;
        private readonly IEmployeeCacheHandler _employeeCacheHandler;

        public UpdateEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            EmployeeManipulator employeeManipulator,
            IEmployeeCacheHandler employeeCacheHandler)
        {
            _employeeRepository = employeeRepository;
            _employeeManipulator = employeeManipulator;
            _employeeCacheHandler = employeeCacheHandler;
        }

        public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            Employee employeeeToBeUpdated = await _employeeRepository.GetByIdAsync(request.Id);

            if (employeeeToBeUpdated == null)
            {
                throw new EntityNotFoundException(typeof(Employee), request.Id);
            }

            employeeeToBeUpdated.SetName(request.Name);
            employeeeToBeUpdated.SetDateOfBirth(request.DateOfBirth);

            await _employeeManipulator.SetDepartmentAsync(employeeeToBeUpdated, request.DepartmentId);
            await _employeeManipulator.SetEmailAsync(employeeeToBeUpdated, request.Email);
            await _employeeManipulator.SetPhoneNumberAsync(employeeeToBeUpdated, request.PhoneNumber);

            await _employeeRepository.UpdateAsync(employeeeToBeUpdated);

            // Remove the cache for this employee
            await _employeeCacheHandler.RemoveDetailsByIdAsync(request.Id);

            return Unit.Value;
        }
    }
}
