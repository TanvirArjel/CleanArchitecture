using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate;

public sealed class Employee : Entity
{
    internal Employee(
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository,
        string name,
        Guid departmentId,
        DateTime dateOfBirth,
        string email,
        string phoneNumber)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetDepartmentId(departmentRepository, departmentId);
        SetDateOfBirth(dateOfBirth);
        SetEmail(employeeRepository, email);
        SetPhoneNumber(employeeRepository, phoneNumber);
        CreatedAtUtc = DateTime.UtcNow;
    }

    // This is needed for EF core query mapping.
    private Employee()
    {
    }

    public string Name { get; private set; }

    public Guid DepartmentId { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public string Email { get; private set; }

    public string PhoneNumber { get; private set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime? LastModifiedAtUtc { get; private set; }

    // Navigation Properties
    public Department Department { get; private set; }

    // Public methods
    public void SetName(string name)
    {
        Name = name.ThrowIfNullOrEmpty(nameof(name))
                   .ThrowIfOutOfLength(2, 50, nameof(name));
    }

    public void SetDateOfBirth(DateTime dateOfBirth)
    {
        DateTime minDateOfBirth = DateTime.UtcNow.AddYears(-115);
        DateTime maxDateOfBirth = DateTime.UtcNow.AddYears(-15);

        // Validate the minimum age.
        dateOfBirth.ThrowIfOutOfRange(minDateOfBirth, maxDateOfBirth, nameof(dateOfBirth), "The minimum age has to be 15 years.");
        DateOfBirth = dateOfBirth;
    }

    public async Task SetDepartmentAsync(IDepartmentRepository repository, Guid departmentId)
    {
        repository.ThrowIfNull(nameof(repository));

        departmentId.ThrowIfEmpty(nameof(departmentId));

        if (DepartmentId != Guid.Empty && DepartmentId.Equals(departmentId))
        {
            return;
        }

        bool isDepartmentExistent = await repository.ExistsAsync(d => d.Id == departmentId);

        if (isDepartmentExistent == false)
        {
            throw new EntityNotFoundException(typeof(Department), departmentId);
        }

        DepartmentId = departmentId;
    }

    public async Task SetEmailAsync(IEmployeeRepository repository, string email)
    {
        repository.ThrowIfNull(nameof(repository));

        email.ThrowIfNullOrEmpty(nameof(email))
                     .ThrowIfNotValidEmail(nameof(email));

        if (Email != null && Email.Equals(email, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        bool isPhoneNumberExistent = await repository.ExistsAsync(d => d.Email == email);

        if (isPhoneNumberExistent)
        {
            throw new DomainValidationException("An employee already exists with the provided email.");
        }

        Email = email;
    }

    public async Task SetPhoneNumberAsync(IEmployeeRepository repository, string phoneNumber)
    {
        repository.ThrowIfNull(nameof(repository));
        phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));

        if (PhoneNumber != null && PhoneNumber.Equals(phoneNumber, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        bool isPhoneNumberExistent = await repository.ExistsAsync(d => d.PhoneNumber == phoneNumber);

        if (isPhoneNumberExistent)
        {
            throw new DomainValidationException("An employee already exists with the provided phone number.");
        }

        PhoneNumber = phoneNumber;
    }

    private void SetDepartmentId(IDepartmentRepository repository, Guid departmentId)
    {
        SetDepartmentAsync(repository, departmentId).GetAwaiter().GetResult();
    }

    private void SetEmail(IEmployeeRepository repository, string email)
    {
        SetEmailAsync(repository, email).GetAwaiter().GetResult();
    }

    private void SetPhoneNumber(IEmployeeRepository employeeRepository, string phoneNumber)
    {
        SetPhoneNumberAsync(employeeRepository, phoneNumber).GetAwaiter().GetResult();
    }
}
