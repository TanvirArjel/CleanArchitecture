using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Aggregates.DepartmentAggregate;

public sealed class Department : Entity, ITimeFields
{
    private Department(Guid id)
    {
        Id = id;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    // This is needed for EF Core query mapping or deserialization.
    private Department()
    {
    }

    public string Name { get; private set; }

    public string Description { get; private set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }

    /// <summary>
    /// This is factory method is for creating a new object of <see cref="Department"/>.
    /// </summary>
    /// <param name="repository">The instance of <see cref="IDepartmentRepository"/>.</param>
    /// <param name="name">The name of the department.</param>
    /// <param name="description">The description for the department.</param>
    /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
    public static async Task<Department> CreateAsync(IDepartmentRepository repository, string name, string description)
    {
        repository.ThrowIfNull(nameof(repository));

        Department department = new Department(Guid.NewGuid());
        await department.SetNameAsync(repository, name);
        department.SetDescription(description);

        return department;
    }

    // Public methods
    public void SetDescription(string description)
    {
        Description = description.ThrowIfNullOrEmpty(nameof(description))
                                 .ThrowIfOutOfLength(20, 100, nameof(description));
    }

    public async Task SetNameAsync(IDepartmentRepository repository, string name)
    {
        repository.ThrowIfNull(nameof(repository));

        name.ThrowIfNullOrEmpty(nameof(name)).ThrowIfOutOfLength(2, 50, nameof(name));

        if (Name != null && Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        bool isNameExistent = await repository.ExistsAsync(d => d.Name == name);

        if (isNameExistent)
        {
            throw new DomainValidationException("A department already exists with the name.");
        }

        Name = name;
    }
}
