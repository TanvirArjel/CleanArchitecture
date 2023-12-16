using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class IsDepartmentExistentByIdQuery(Guid departmentId) : IRequest<bool>
{
    public Guid Id { get; } = departmentId.ThrowIfEmpty(nameof(departmentId));
}

internal class IsDepartmentExistentByIdQueryHandler(IQueryRepository repository) : IRequestHandler<IsDepartmentExistentByIdQuery, bool>
{
    private readonly IQueryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<bool> Handle(IsDepartmentExistentByIdQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));

        bool isExists = await _repository.ExistsAsync<Department>(d => d.Id == request.Id, cancellationToken);
        return isExists;
    }
}
