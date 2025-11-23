using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Commands.EmployeeCommands;

public record CreateEmployeeCommand(
    string FirstName,
    string LastName,
    Guid DepartmentId,
    DateTime DateOfBirth,
    string Email,
    string PhoneNumber) : IRequest<Result<Guid>>;

internal class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IEmployeeCacheHandler _employeeCacheHandler;

    public CreateEmployeeCommandHandler(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IEmployeeCacheHandler employeeCacheHandler)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
        _employeeCacheHandler = employeeCacheHandler ?? throw new ArgumentNullException(nameof(employeeCacheHandler));
    }

    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Result<Employee> result = await Employee.CreateAsync(
            _departmentRepository,
            _employeeRepository,
            request.FirstName,
            request.LastName,
            request.DepartmentId,
            request.DateOfBirth,
            request.Email,
            request.PhoneNumber);

        if (result.IsSuccess == false)
        {
            return Result<Guid>.Failure(result.Errors);
        }

        // Persist to the database
        await _employeeRepository.InsertAsync(result.Value);

        return Result<Guid>.Success(result.Value.Id);
    }
}