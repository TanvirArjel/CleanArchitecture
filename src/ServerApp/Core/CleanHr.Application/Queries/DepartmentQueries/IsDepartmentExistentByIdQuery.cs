using CleanHr.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace CleanHr.Application.Queries.DepartmentQueries;

public sealed class IsDepartmentExistentByIdQuery(Guid departmetnId) : IRequest<bool>
{
    public Guid Id { get; } = departmetnId.ThrowIfEmpty(nameof(departmetnId));

    private class IsDepartmentExistentByIdQueryHandler(IQueryRepository repository) : IRequestHandler<IsDepartmentExistentByIdQuery, bool>
    {

        public async Task<bool> Handle(IsDepartmentExistentByIdQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));

            bool isExists = await repository.ExistsAsync<Department>(d => d.Id == request.Id, cancellationToken);
            return isExists;
        }
    }
}
