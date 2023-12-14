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

    private class GetEmployeeListQueryHandler(IQueryRepository repository) : IRequestHandler<GetEmployeeListQuery, PaginatedList<EmployeeDto>>
    {
        public async Task<PaginatedList<EmployeeDto>> Handle(GetEmployeeListQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            Expression<Func<Employee, EmployeeDto>> selectExpression = e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name.FirstName + " " + e.Name.LastName,
                DepartmentId = e.DepartmentId,
                DepartmentName = e.Department.Name.Value,
                DateOfBirth = e.DateOfBirth.Value,
                Email = e.Email.Value,
                PhoneNumber = e.PhoneNumber.Value,
                IsActive = e.IsActive,
                CreatedAtUtc = e.CreatedAtUtc,
                LastModifiedAtUtc = e.LastModifiedAtUtc
            };

            PaginationSpecification<Employee> paginationSpecification = new()
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            PaginatedList<EmployeeDto> employeeDetailsDtos = await repository.GetListAsync(paginationSpecification, selectExpression, cancellationToken);

            return employeeDetailsDtos;
        }
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
