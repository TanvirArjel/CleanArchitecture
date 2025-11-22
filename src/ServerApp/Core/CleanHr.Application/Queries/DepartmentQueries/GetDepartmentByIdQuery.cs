using System.Linq.Expressions;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class GetDepartmentByIdQuery(Guid id) : IRequest<DepartmentDetailsDto>
{
    public Guid Id { get; } = id.ThrowIfEmpty(nameof(id));
}

public class DepartmentDetailsDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }
}

// Handler
internal class GetDepartmentByIdQueryHandler(IQueryRepository repository) : IRequestHandler<GetDepartmentByIdQuery, DepartmentDetailsDto>
{
    private readonly IQueryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<DepartmentDetailsDto> Handle(GetDepartmentByIdQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        Expression<Func<Department, DepartmentDetailsDto>> selectExp = d => new DepartmentDetailsDto
        {
            Id = d.Id,
            Name = d.Name,
            Description = d.Description,
            IsActive = d.IsActive,
            CreatedAtUtc = d.CreatedAtUtc,
            LastModifiedAtUtc = d.LastModifiedAtUtc
        };

        DepartmentDetailsDto departmentDetailsDto = await _repository.GetByIdAsync(request.Id, selectExp, cancellationToken);

        return departmentDetailsDto;
    }
}
