using System;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate
{
    [ScopedService]
    public class DepartmentManipulator
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentManipulator(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<Department> SetNameAsync(Department department, string name)
        {
            department.ThrowIfNull(nameof(department));

            if (department.Name.Equals(name))
            {
                return department;
            }

            bool isNameExistent = await _departmentRepository.ExistsAsync(d => d.Name == name);

            if (isNameExistent)
            {
                throw new InvalidOperationException("A department already exists with the name.");
            }

            department.SetName(name);
            return department;
        }
    }
}
