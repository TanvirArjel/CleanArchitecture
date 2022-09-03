using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.ValueObjects;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

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

    public DepartmentName Name { get; private set; }

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
    public static async Task<Department> CreateAsync(
        IDepartmentRepository repository,
        DepartmentName name,
        string description)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        Department department = new Department(Guid.NewGuid());
        await department.SetNameAsync(repository, name);
        department.SetDescription(description);

        return department;
    }

    // Public methods
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainValidationException("The Department description cannot be null or empty.");
        }

        if (description.Length < 20 || description.Length > 100)
        {
            throw new DomainValidationException("The Department description cannot be null or empty.");
        }

        Description = description;
    }

    public async Task SetNameAsync(IDepartmentRepository repository, DepartmentName name)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (name == null)
        {
            throw new DomainValidationException("The name cannot be null.");
        }

        if (Name != null && Name.Value.Equals(name.Value, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        bool isNameExistent = await repository.ExistsAsync(d => d.Name.Value == name.Value);

        if (isNameExistent)
        {
            throw new DomainValidationException("A department already exists with the name.");
        }

        Name = name;
    }
}
