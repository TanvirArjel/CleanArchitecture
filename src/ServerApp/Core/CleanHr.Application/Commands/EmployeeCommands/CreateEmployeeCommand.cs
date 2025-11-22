using CleanHr.Application.Caching.Handlers;
using CleanHr.Domain;
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

internal class CreateEmployeeCommandHandler(
    IEmployeeRepository employeeRepository,
    IEmployeeCacheHandler employeeCacheHandler,
    EmployeeFactory employeeFactory) : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    private readonly IEmployeeRepository _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    private readonly IEmployeeCacheHandler _employeeCacheHandler = employeeCacheHandler ?? throw new ArgumentNullException(nameof(employeeCacheHandler));
    private readonly EmployeeFactory _employeeFactory = employeeFactory ?? throw new ArgumentNullException(nameof(employeeFactory));

    public async Task<Result<Guid>> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Result<Employee> result = await _employeeFactory.CreateAsync(
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
