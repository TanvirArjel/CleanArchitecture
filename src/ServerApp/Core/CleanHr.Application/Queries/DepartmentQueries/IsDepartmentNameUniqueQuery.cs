using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class IsDepartmentNameUniqueQuery(Guid departmentId, string name) : IRequest<bool>
{
    public Guid Id { get; } = departmentId;

    public string Name { get; } = name.ThrowIfNullOrEmpty(nameof(name));
}

internal class IsDepartmentNameUniqueQueryHandler(
        IQueryRepository repository) : IRequestHandler<IsDepartmentNameUniqueQuery, bool>
{
    private readonly IQueryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<bool> Handle(IsDepartmentNameUniqueQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        bool isExistent = await _repository.ExistsAsync<Department>(d => d.Id != request.Id && d.Name == request.Name, cancellationToken);
        return !isExistent;
    }
}
