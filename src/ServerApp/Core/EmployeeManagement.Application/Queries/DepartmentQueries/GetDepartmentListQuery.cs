using EmployeeManagement.Application.Caching.Repositories;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Application.Queries.DepartmentQueries;

public class GetDepartmentListQuery : IRequest<List<DepartmentDto>>
{
    private class GetDepartmentListQueryHandler : IRequestHandler<GetDepartmentListQuery, List<DepartmentDto>>
    {
        private readonly IDepartmentCacheRepository _departmentCacheRepository;

        public GetDepartmentListQueryHandler(IDepartmentCacheRepository departmentCacheRepository)
        {
            _departmentCacheRepository = departmentCacheRepository;
        }

        public async Task<List<DepartmentDto>> Handle(GetDepartmentListQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            List<DepartmentDto> departmentDtos = await _departmentCacheRepository.GetListAsync();
            return departmentDtos;
        }
    }
}

public class DepartmentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }
}
