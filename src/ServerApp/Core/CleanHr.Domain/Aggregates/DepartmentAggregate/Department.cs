using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanHr.Domain.Exceptions;
using CleanHr.Domain.Primitives;
using CleanHr.Domain.ValueObjects;

namespace CleanHr.Domain.Aggregates.DepartmentAggregate;

public sealed class Department : AggregateRoot, ITimeFields
{
    private Department(Guid id)
    {
        Id = id;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    // This is needed for EF Core query mapping or deserialization.
    [JsonConstructor]
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
        ArgumentNullException.ThrowIfNull(repository);

        Department department = new(Guid.NewGuid());
        await department.SetNameAsync(repository, name);
        department.SetDescription(description);

        return department;
    }

    // Public methods
    public void SetDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new DomainValidationException(DepartmentDomainErrors.DescriptionNullOrEmpty);
        }

        if (description.Length < 20 || description.Length > 100)
        {
            throw new DomainValidationException(DepartmentDomainErrors.GetDescriptionLengthOutOfRangeMessage(20, 200));
        }

        Description = description;
    }

    public async Task SetNameAsync(IDepartmentRepository repository, DepartmentName name)
    {
        ArgumentNullException.ThrowIfNull(repository);

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
