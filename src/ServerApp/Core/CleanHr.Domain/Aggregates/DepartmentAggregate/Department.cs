using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate.Validators;
using CleanHr.Domain.Primitives;
using FluentValidation.Results;

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
    public static async Task<Result<Department>> CreateAsync(
        IDepartmentRepository repository,
        string name,
        string description)
    {
        ArgumentNullException.ThrowIfNull(repository);

        DepartmentValidator validator = new();
        UniqueDepartmentNameValidator uniqueNameValidator = new(repository);

        Department department = new(Guid.NewGuid())
        {
            Name = name,
            Description = description
        };

        ValidationResult validationResult = await validator.ValidateAsync(department);

        if (validationResult.IsValid == false)
        {
            return Result<Department>.Failure(validationResult.ToDictionary());
        }

        ValidationResult uniqueNameResult = await uniqueNameValidator.ValidateAsync(department);

        if (uniqueNameResult.IsValid == false)
        {
            return Result<Department>.Failure(uniqueNameResult.ToDictionary());
        }

        return Result<Department>.Success(department);
    }

    // Public methods
    public Result SetDescription(string description)
    {
        string originalDescription = Description;
        Description = description;

        DepartmentValidator validator = new();
        ValidationResult validationResult = validator.Validate(this);

        if (validationResult.IsValid == false)
        {
            Description = originalDescription;
            return Result.Failure(validationResult.ToDictionary());
        }

        return Result.Success();
    }

    public async Task<Result> SetNameAsync(IDepartmentRepository repository, string name)
    {
        ArgumentNullException.ThrowIfNull(repository);

        string originalName = Name;
        Name = name;

        DepartmentValidator validator = new();
        UniqueDepartmentNameValidator uniqueNameValidator = new(repository);

        ValidationResult result = await validator.ValidateAsync(this);

        if (result.IsValid == false)
        {
            Name = originalName;
            return Result.Failure(result.ToDictionary());
        }

        ValidationResult uniqueNameResult = await uniqueNameValidator.ValidateAsync(this);

        if (uniqueNameResult.IsValid == false)
        {
            Name = originalName;
            return Result.Failure(uniqueNameResult.ToDictionary());
        }

        return Result.Success();
    }
}
