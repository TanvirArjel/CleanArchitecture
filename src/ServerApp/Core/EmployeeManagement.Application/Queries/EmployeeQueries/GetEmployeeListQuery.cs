using System.Linq.Expressions;
using EmployeeManagement.Domain.Aggregates.EmployeeAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Queries.EmployeeQueries;

public class GetEmployeeListQuery : IRequest<PaginatedList<EmployeeDto>>
{
    public GetEmployeeListQuery(int pageIndex, int pageSize)
    {
        PageIndex = pageIndex.ThrowIfZeroOrNegative(nameof(pageIndex));
        PageSize = pageSize.ThrowIfOutOfRange(1, 50, nameof(pageSize));
    }

    public int PageIndex { get; }

    public int PageSize { get; }

    private class GetEmployeeListQueryHandler : IRequestHandler<GetEmployeeListQuery, PaginatedList<EmployeeDto>>
    {
        private readonly IQueryRepository _repository;

        public GetEmployeeListQueryHandler(IQueryRepository repository)
        {
            _repository = repository;
        }

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

            PaginationSpecification<Employee> paginationSpecification = new PaginationSpecification<Employee>
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };

            PaginatedList<EmployeeDto> employeeDetailsDtos = await _repository.GetListAsync(paginationSpecification, selectExpression, cancellationToken);

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
