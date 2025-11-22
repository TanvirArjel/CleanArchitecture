using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class IsDepartmentExistentByNameQuery(string name) : IRequest<bool>
{
    public string Name { get; set; } = name.ThrowIfNullOrEmpty(nameof(name));
}

internal class IsDepartmentExistentByNameQueryHandler(IQueryRepository repository) : IRequestHandler<IsDepartmentExistentByNameQuery, bool>
{
    private readonly IQueryRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public async Task<bool> Handle(IsDepartmentExistentByNameQuery request, CancellationToken cancellationToken)
    {
        request.ThrowIfNull(nameof(request));
        bool isExists = await _repository.ExistsAsync<Department>(d => d.Name == request.Name, cancellationToken);
        return isExists;
    }
}
