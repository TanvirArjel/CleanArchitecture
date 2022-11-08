using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class IsDepartmentNameUniqueQuery : IRequest<bool>
{
    public IsDepartmentNameUniqueQuery(Guid departmentId, string name)
    {
        Id = departmentId.ThrowIfEmpty(nameof(departmentId));
        Name = name.ThrowIfNullOrEmpty(nameof(name));
    }

    public Guid Id { get; }

    public string Name { get; }

    private class IsDepartmentNameUniqueQueryHandler : IRequestHandler<IsDepartmentNameUniqueQuery, bool>
    {
        private readonly IQueryRepository _repository;

        public IsDepartmentNameUniqueQueryHandler(IQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(IsDepartmentNameUniqueQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExistent = await _repository.ExistsAsync<Department>(d => d.Id != request.Id && d.Name.Value == request.Name, cancellationToken);
            return !isExistent;
        }
    }
}
