using CleanHr.Application.Caching.Repositories;
using MediatR;
using TanvirArjel.ArgumentChecker;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class GetDepartmentListQuery : IRequest<List<DepartmentDto>>
{
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

internal class GetDepartmentListQueryHandler(IDepartmentCacheRepository departmentCacheRepository) : IRequestHandler<GetDepartmentListQuery, List<DepartmentDto>>
{
    private readonly IDepartmentCacheRepository _departmentCacheRepository = departmentCacheRepository ?? throw new ArgumentNullException(nameof(departmentCacheRepository));

    public async Task<List<DepartmentDto>> Handle(GetDepartmentListQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        List<DepartmentDto> departmentDtos = await _departmentCacheRepository.GetListAsync();
        return departmentDtos;
    }
}
