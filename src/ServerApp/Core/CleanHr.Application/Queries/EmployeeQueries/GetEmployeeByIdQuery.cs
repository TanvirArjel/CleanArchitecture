using CleanHr.Application.Caching.Repositories;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Queries.EmployeeQueries;

public sealed class GetEmployeeByIdQuery : IRequest<EmployeeDetailsDto>
{
    public GetEmployeeByIdQuery(Guid employeeId)
    {
        Id = employeeId.ThrowIfEmpty(nameof(employeeId));
    }

    public Guid Id { get; }

    private class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailsDto>
    {
        private readonly IEmployeeCacheRepository _employeeCacheRepository;

        public GetEmployeeByIdQueryHandler(IEmployeeCacheRepository employeeCacheRepository)
        {
            _employeeCacheRepository = employeeCacheRepository;
        }

        public async Task<EmployeeDetailsDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            EmployeeDetailsDto employeeDetailsDto = await _employeeCacheRepository.GetDetailsByIdAsync(request.Id);
            return employeeDetailsDto;
        }
    }
}

public class EmployeeDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid DepartmentId { get; set; }

    public string DepartmentName { get; set; }

    public DateTime DateOfBirth { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }
}
