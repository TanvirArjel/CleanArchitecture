using System;
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

        DepartmentNameValidator nameValidator = new(repository, Guid.Empty);
        ValidationResult nameValidationResult = await nameValidator.ValidateAsync(name);

        if (nameValidationResult.IsValid == false)
        {
            return Result<Department>.Failure(nameValidationResult.ToDictionary());
        }

        DepartmentDescriptionValidator descriptionValidator = new();
        ValidationResult validationResult = await descriptionValidator.ValidateAsync(description);

        if (validationResult.IsValid == false)
        {
            return Result<Department>.Failure(validationResult.ToDictionary());
        }

        Department department = new(Guid.NewGuid())
        {
            Name = name,
            Description = description
        };

        return Result<Department>.Success(department);
    }

    // Public methods
    public Result SetDescription(string description)
    {
        DepartmentDescriptionValidator validator = new();
        ValidationResult validationResult = validator.Validate(description);

        if (validationResult.IsValid == false)
        {
            return Result.Failure(validationResult.ToDictionary());
        }

        Description = description;
        LastModifiedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public async Task<Result> SetNameAsync(IDepartmentRepository repository, string name)
    {
        ArgumentNullException.ThrowIfNull(repository);

        DepartmentNameValidator nameValidator = new(repository, this.Id);

        ValidationResult result = await nameValidator.ValidateAsync(name);

        if (result.IsValid == false)
        {
            return Result.Failure(result.ToDictionary());
        }

        Name = name;
        LastModifiedAtUtc = DateTime.UtcNow;

        return Result.Success();
    }
}
