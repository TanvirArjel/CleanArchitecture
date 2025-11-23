using System;
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

        EmployeeNameValidator firstNameValidator = new("FirstName");
        ValidationResult firstNameValidationResult = await firstNameValidator.ValidateAsync(firstName);

        if (firstNameValidationResult.IsValid == false)
        {
            return Result<Employee>.Failure(firstNameValidationResult.ToDictionary());
        }

        EmployeeNameValidator lastNameValidator = new("LastName");
        ValidationResult lastNameValidationResult = await lastNameValidator.ValidateAsync(lastName);
        if (lastNameValidationResult.IsValid == false)
        {
            return Result<Employee>.Failure(lastNameValidationResult.ToDictionary());
        }

        EmployeeDepartmentValidator departmentValidator = new(departmentRepository);
        ValidationResult departmentValidationResult = await departmentValidator.ValidateAsync(departmentId);
        if (departmentValidationResult.IsValid == false)
        {
            return Result<Employee>.Failure(departmentValidationResult.ToDictionary());
        }

        EmployeeDateOfBirthValidator dateOfBirthValidator = new();
        ValidationResult dateOfBirthValidationResult = await dateOfBirthValidator.ValidateAsync(dateOfBirth);
        if (dateOfBirthValidationResult.IsValid == false)
        {
            return Result<Employee>.Failure(dateOfBirthValidationResult.ToDictionary());
        }

        EmployeeEmailValidator emailValidator = new(employeeRepository, Guid.Empty);
        ValidationResult emailValidationResult = await emailValidator.ValidateAsync(email);

        if (emailValidationResult.IsValid == false)
        {
            return Result<Employee>.Failure(emailValidationResult.ToDictionary());
        }

        PhoneNumberValidator phoneNumberValidator = new(employeeRepository, Guid.Empty);
        ValidationResult phoneNumberValidationResult = await phoneNumberValidator.ValidateAsync(phoneNumber);

        if (phoneNumberValidationResult.IsValid == false)
        {
            return Result<Employee>.Failure(phoneNumberValidationResult.ToDictionary());
        }


        Employee employee = new(Guid.NewGuid())
        {
            FirstName = firstName,
            LastName = lastName,
            DepartmentId = departmentId,
            DateOfBirth = dateOfBirth,
            Email = email,
            PhoneNumber = phoneNumber
        };

        return Result<Employee>.Success(employee);
    }

    // Public methods
    public Result SetName(string firstName, string lastName)
    {
        EmployeeNameValidator firstNameValidator = new("FirstName");
        ValidationResult firstNameValidationResult = firstNameValidator.Validate(firstName);


        if (firstNameValidationResult.IsValid == false)
        {
            return Result.Failure(firstNameValidationResult.ToDictionary());
        }

        EmployeeNameValidator lastNameValidator = new("LastName");
        ValidationResult lastNameValidationResult = lastNameValidator.Validate(lastName);

        if (lastNameValidationResult.IsValid == false)
        {
            return Result.Failure(lastNameValidationResult.ToDictionary());
        }

        FirstName = firstName;
        LastName = lastName;

        return Result.Success();
    }

    public Result SetDateOfBirth(DateTime dateOfBirth)
    {
        DateTime originalDateOfBirth = DateOfBirth;
        DateOfBirth = dateOfBirth;

        EmployeeDateOfBirthValidator validator = new();
        ValidationResult validationResult = validator.Validate(dateOfBirth);

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

        EmployeeDepartmentValidator validator = new(repository);
        ValidationResult validationResult = await validator.ValidateAsync(departmentId);

        if (validationResult.IsValid == false)
        {
            return Result.Failure(validationResult.ToDictionary());
        }

        DepartmentId = departmentId;
        return Result.Success();
    }

    public async Task<Result> SetEmailAsync(IEmployeeRepository repository, string email)
    {
        ArgumentNullException.ThrowIfNull(repository);

        EmployeeEmailValidator validator = new(repository, Id);
        ValidationResult validationResult = await validator.ValidateAsync(email);

        if (validationResult.IsValid == false)
        {
            return Result.Failure(validationResult.ToDictionary());
        }

        Email = email;
        return Result.Success();
    }

    public async Task<Result> SetPhoneNumberAsync(IEmployeeRepository repository, string phoneNumber)
    {
        ArgumentNullException.ThrowIfNull(repository);

        PhoneNumberValidator validator = new(repository, Id);
        ValidationResult validationResult = await validator.ValidateAsync(phoneNumber);

        if (validationResult.IsValid == false)
        {
            return Result.Failure(validationResult.ToDictionary());
        }

        PhoneNumber = phoneNumber;
        return Result.Success();
    }
}
