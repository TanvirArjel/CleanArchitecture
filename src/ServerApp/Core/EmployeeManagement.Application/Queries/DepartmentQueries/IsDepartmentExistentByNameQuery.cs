using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Queries.DepartmentQueries;

public class IsDepartmentExistentByNameQuery : IRequest<bool>
{
    public IsDepartmentExistentByNameQuery(string name)
    {
        Name = name.ThrowIfNullOrEmpty(nameof(name));
    }

    public string Name { get; set; }

    private class IsDepartmentExistentByNameQueryHandler : IRequestHandler<IsDepartmentExistentByNameQuery, bool>
    {
        private readonly IQueryRepository _repository;

        public IsDepartmentExistentByNameQueryHandler(IQueryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(IsDepartmentExistentByNameQuery request, CancellationToken cancellationToken)
        {
            request.ThrowIfNull(nameof(request));
            bool isExists = await _repository.ExistsAsync<Department>(d => d.Name.Value == request.Name, cancellationToken);
            return isExists;
        }
    }
}
