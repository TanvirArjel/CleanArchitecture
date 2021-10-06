using System.Threading;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using MediatR;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;

namespace EmployeeManagement.Application.Queries.DepartmentQueries
{
    public class IsDepartmentExistentByNameQuery : IRequest<bool>
    {
        public IsDepartmentExistentByNameQuery(string name)
        {
            Name = name.ThrowIfNullOrEmpty(nameof(name));
        }

        public string Name { get; set; }

        private class IsDepartmentExistentByNameQueryHandler : IRequestHandler<IsDepartmentExistentByNameQuery, bool>
        {
            private readonly IRepository _repository;

            public IsDepartmentExistentByNameQueryHandler(IRepository repository)
            {
                _repository = repository;
            }

            public async Task<bool> Handle(IsDepartmentExistentByNameQuery request, CancellationToken cancellationToken)
            {
                request.ThrowIfNull(nameof(request));
                bool isExists = await _repository.ExistsAsync<Department>(d => d.Name == request.Name);
                return isExists;
            }
        }
    }
}
