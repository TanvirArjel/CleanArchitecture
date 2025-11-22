using System.Linq.Expressions;
using CleanHr.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.EmployeeQueries;

public sealed class GetEmployeeListQuery(int pageIndex, int pageSize) : IRequest<PaginatedList<EmployeeDto>>
{
    public int PageIndex { get; } = pageIndex.ThrowIfZeroOrNegative(nameof(pageIndex));

    public int PageSize { get; } = pageSize.ThrowIfOutOfRange(1, 50, nameof(pageSize));
}

internal class GetEmployeeListQueryHandler(IQueryRepository repository) : IRequestHandler<GetEmployeeListQuery, PaginatedList<EmployeeDto>>
{
    private readonly IQueryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<PaginatedList<EmployeeDto>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Expression<Func<Employee, EmployeeDto>> selectExpression = e => new EmployeeDto
        {
            Id = e.Id,
            Name = e.FirstName + " " + e.LastName,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department.Name,
            DateOfBirth = e.DateOfBirth,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber,
            IsActive = e.IsActive,
            CreatedAtUtc = e.CreatedAtUtc,
            LastModifiedAtUtc = e.LastModifiedAtUtc
        };

        PaginationSpecification<Employee> paginationSpecification = new()
        {
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };

        PaginatedList<EmployeeDto> employeeDetailsDtos = await _repository.GetListAsync(paginationSpecification, selectExpression, cancellationToken);

        return employeeDetailsDtos;
    }
}

public class EmployeeDto
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
