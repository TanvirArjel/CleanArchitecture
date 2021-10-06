using System;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.EFCore.GenericRepository;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate
{
    [ScopedService]
    public class DepartmentManipulator
    {
        private readonly IRepository _repository;

        public DepartmentManipulator(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Department> SetNameAsync(Department department, string name)
        {
            department.ThrowIfNull(nameof(department));

            if (department.Name.Equals(name))
            {
                return department;
            }

            bool isNameExistent = await _repository.ExistsAsync<Department>(d => d.Name == name);

            if (isNameExistent)
            {
                throw new InvalidOperationException("A department already exists with the name.");
            }

            department.SetName(name);
            return department;
        }
    }
}
