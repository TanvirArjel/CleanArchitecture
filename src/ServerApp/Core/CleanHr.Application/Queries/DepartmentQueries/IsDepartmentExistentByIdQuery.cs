using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class IsDepartmentExistentByIdQuery : IRequest<bool>
{
    public IsDepartmentExistentByIdQuery(Guid departmetnId)
    {
        Id = departmetnId.ThrowIfEmpty(nameof(departmetnId));
    }

    public Guid Id { get; }

    private class IsDepartmentExistentByIdQueryHandler : IRequestHandler<IsDepartmentExistentByIdQuery, bool>
    {
        private readonly IQueryRepository _repository;

        public IsDepartmentExistentByIdQueryHandler(IQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(IsDepartmentExistentByIdQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExists = await _repository.ExistsAsync<Department>(d => d.Id == request.Id, cancellationToken);
            return isExists;
        }
    }
}
