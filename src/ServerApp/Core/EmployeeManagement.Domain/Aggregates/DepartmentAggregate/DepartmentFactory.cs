using System;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate
{
    [ScopedService]
    public class DepartmentFactory
    {
        private readonly IRepository _repository;

        public DepartmentFactory(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Department> CreateAsync(string name, string description)
        {
            name.ThrowIfNullOrEmpty(nameof(name))
                       .ThrowIfOutOfLength(2, 50, nameof(name));

            description.ThrowIfNullOrEmpty(nameof(description))
                                     .ThrowIfOutOfLength(20, 100, nameof(description));

            bool isNameExistent = await _repository.ExistsAsync<Department>(d => d.Name == name);

            if (isNameExistent)
            {
                throw new InvalidOperationException("A department already exists with the name.");
            }

            Department department = new Department(name, description);

            return department;
        }
    }
}
