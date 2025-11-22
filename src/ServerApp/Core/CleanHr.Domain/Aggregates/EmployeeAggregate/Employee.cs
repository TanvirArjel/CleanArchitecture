using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;
using CleanHr.Domain.Primitives;
using FluentValidation.Results;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate;

public sealed class Employee : AggregateRoot
{
    private Employee(Guid id)
    {
        Id = id;
        IsActive = true;
        CreatedAtUtc = DateTime.UtcNow;
    }

    // This is needed for EF core query mapping and serialization.
    [JsonConstructor]
    private Employee()
    {
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public Guid DepartmentId { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? LastModifiedAtUtc { get; set; }

    // Navigation Properties
    public Department Department { get; private set; }

    /// <summary>
    /// This is factory method is for creating a new object of <see cref="Employee"/>.
    /// </summary>
    /// <param name="departmentRepository">The instance of <see cref="IDepartmentRepository"/>.</param>
    /// <param name="employeeRepository">The instance of <see cref="IEmployeeRepository"/>.</param>
    /// <param name="firstName">The first name of the employee.</param>
    /// <param name="lastName">The last name of the employee.</param>
    /// <param name="departmentId">The department id.</param>
    /// <param name="dateOfBirth">The date of birth.</param>
    /// <param name="email">The email address.</param>
    /// <param name="phoneNumber">The phone number.</param>
    /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
    public static async Task<Result<Employee>> CreateAsync(
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository,
        string firstName,
        string lastName,
        Guid departmentId,
        DateTime dateOfBirth,
        string email,
        string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(departmentRepository);
        ArgumentNullException.ThrowIfNull(employeeRepository);

        // Check if department exists
        bool isDepartmentExistent = await departmentRepository.ExistsAsync(d => d.Id == departmentId);
        if (isDepartmentExistent == false)
        {
            return Result<Employee>.Failure(new Dictionary<string, string>
            {
                { nameof(departmentId), $"The Department does not exist with the id value: {departmentId}" }
            });
        }

        EmployeeValidator validator = new();
        UniqueEmployeeEmailValidator uniqueEmailValidator = new(employeeRepository);
        UniqueEmployeePhoneNumberValidator uniquePhoneNumberValidator = new(employeeRepository);

        Employee employee = new(Guid.NewGuid())
        {
            FirstName = firstName,
            LastName = lastName,
            DepartmentId = departmentId,
            DateOfBirth = dateOfBirth,
            Email = email,
            PhoneNumber = phoneNumber
        };

        ValidationResult validationResult = await validator.ValidateAsync(employee);

        if (validationResult.IsValid == false)
        {
            return Result<Employee>.Failure(validationResult.ToDictionary());
        }

        ValidationResult uniqueEmailResult = await uniqueEmailValidator.ValidateAsync(employee);

        if (uniqueEmailResult.IsValid == false)
        {
            return Result<Employee>.Failure(uniqueEmailResult.ToDictionary());
        }

        ValidationResult uniquePhoneNumberResult = await uniquePhoneNumberValidator.ValidateAsync(employee);

        if (uniquePhoneNumberResult.IsValid == false)
        {
            return Result<Employee>.Failure(uniquePhoneNumberResult.ToDictionary());
        }

        return Result<Employee>.Success(employee);
    }

    // Public methods
    public Result SetName(string firstName, string lastName)
    {
        string originalFirstName = FirstName;
        string originalLastName = LastName;
        FirstName = firstName;
        LastName = lastName;

        EmployeeValidator validator = new();
        ValidationResult validationResult = validator.Validate(this);

        if (validationResult.IsValid == false)
        {
            FirstName = originalFirstName;
            LastName = originalLastName;
            return Result.Failure(validationResult.ToDictionary());
        }

        return Result.Success();
    }

    public Result SetDateOfBirth(DateTime dateOfBirth)
    {
        DateTime originalDateOfBirth = DateOfBirth;
        DateOfBirth = dateOfBirth;

        EmployeeValidator validator = new();
        ValidationResult validationResult = validator.Validate(this);

        if (validationResult.IsValid == false)
        {
            DateOfBirth = originalDateOfBirth;
            return Result.Failure(validationResult.ToDictionary());
        }

        return Result.Success();
    }

    public async Task<Result> SetDepartmentAsync(IDepartmentRepository repository, Guid departmentId)
    {
        ArgumentNullException.ThrowIfNull(repository);

        if (departmentId == Guid.Empty)
        {
            return Result.Failure(new Dictionary<string, string>
            {
                { nameof(departmentId), "The departmentId cannot be empty guid." }
            });
        }

        if (DepartmentId != Guid.Empty && DepartmentId.Equals(departmentId))
        {
            return Result.Success();
        }

        bool isDepartmentExistent = await repository.ExistsAsync(d => d.Id == departmentId);

        if (isDepartmentExistent == false)
        {
            return Result.Failure(new Dictionary<string, string>
            {
                { nameof(departmentId), $"The Department does not exist with the id value: {departmentId}" }
            });
        }

        DepartmentId = departmentId;
        return Result.Success();
    }

    public async Task<Result> SetEmailAsync(IEmployeeRepository repository, string email)
    {
        ArgumentNullException.ThrowIfNull(repository);

        string originalEmail = Email;
        Email = email;

        EmployeeValidator validator = new();
        UniqueEmployeeEmailValidator uniqueEmailValidator = new(repository);

        ValidationResult validationResult = await validator.ValidateAsync(this);

        if (validationResult.IsValid == false)
        {
            Email = originalEmail;
            return Result.Failure(validationResult.ToDictionary());
        }

        ValidationResult uniqueEmailResult = await uniqueEmailValidator.ValidateAsync(this);

        if (uniqueEmailResult.IsValid == false)
        {
            Email = originalEmail;
            return Result.Failure(uniqueEmailResult.ToDictionary());
        }

        return Result.Success();
    }

    public async Task<Result> SetPhoneNumberAsync(IEmployeeRepository repository, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(repository);

        string originalPhoneNumber = PhoneNumber;
        PhoneNumber = phoneNumber;

        EmployeeValidator validator = new();
        UniqueEmployeePhoneNumberValidator uniquePhoneNumberValidator = new(repository);

        ValidationResult validationResult = await validator.ValidateAsync(this);

        if (validationResult.IsValid == false)
        {
            PhoneNumber = originalPhoneNumber;
            return Result.Failure(validationResult.ToDictionary());
        }

        ValidationResult uniquePhoneNumberResult = await uniquePhoneNumberValidator.ValidateAsync(this);

        if (uniquePhoneNumberResult.IsValid == false)
        {
            PhoneNumber = originalPhoneNumber;
            return Result.Failure(uniquePhoneNumberResult.ToDictionary());
        }

        return Result.Success();
    }
}
