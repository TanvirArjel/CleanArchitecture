using System;
using System.Threading.Tasks;
using TanvirArjel.ArgumentChecker;
using TanvirArjel.Extensions.Microsoft.DependencyInjection;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate;

[ScopedService]
public class DepartmentFactory
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentFactory(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Department> CreateAsync(string name, string description)
    {
        name.ThrowIfNullOrEmpty(nameof(name));
        description.ThrowIfNullOrEmpty(nameof(description));

        bool isNameExistent = await _departmentRepository.ExistsAsync(d => d.Name == name);

        if (isNameExistent)
        {
            throw new InvalidOperationException($"A department already exists with the name {name}.");
        }

        Department department = new Department(name, description);

        return department;
    }
}
