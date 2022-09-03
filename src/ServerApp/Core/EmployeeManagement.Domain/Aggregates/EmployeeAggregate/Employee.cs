using System;
using System.Threading.Tasks;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Aggregates.ValueObjects;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate;

public sealed class Employee : Entity
{
    internal Employee(
        IDepartmentRepository departmentRepository,
        IEmployeeRepository employeeRepository,
        Name name,
        Guid departmentId,
        DateOfBirth dateOfBirth,
        Email email,
        PhoneNumber phoneNumber)
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

    public Name Name { get; private set; }

    public Guid DepartmentId { get; private set; }

    public DateOfBirth DateOfBirth { get; private set; }

    public Email Email { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; }

    public DateTime? LastModifiedAtUtc { get; private set; }

    // Navigation Properties
    public Department Department { get; private set; }

    // Public methods
    public void SetName(Name name)
    {
        Name = name ?? throw new DomainValidationException("The name cannot be null.");
    }

    public void SetDateOfBirth(DateOfBirth dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }

    public async Task SetDepartmentAsync(IDepartmentRepository repository, Guid departmentId)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (departmentId == Guid.Empty)
        {
            throw new DomainValidationException("The departmentId cannot be empty guid.");
        }

        if (DepartmentId != Guid.Empty && DepartmentId.Equals(departmentId))
        {
            return;
        }

        bool isDepartmentExistent = await repository.ExistsAsync(d => d.Id == departmentId);

        if (isDepartmentExistent == false)
        {
            throw new DomainValidationException($"The Department does not exist with the id value: {departmentId}");
        }

        DepartmentId = departmentId;
    }

    public async Task SetEmailAsync(IEmployeeRepository repository, Email email)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (email == null)
        {
            throw new DomainValidationException("The email cannot be null.");
        }

        if (Email != null && Email.Value.Equals(email.Value, StringComparison.OrdinalIgnoreCase))
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

    public async Task SetPhoneNumberAsync(IEmployeeRepository repository, PhoneNumber phoneNumber)
    {
        if (repository is null)
        {
            throw new ArgumentNullException(nameof(repository));
        }

        if (phoneNumber == null)
        {
            throw new DomainValidationException("The phoneNumber cannot be null.");
        }

        if (PhoneNumber != null && PhoneNumber.Value.Equals(phoneNumber.Value, StringComparison.OrdinalIgnoreCase))
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

    private void SetEmail(IEmployeeRepository repository, Email email)
    {
        SetEmailAsync(repository, email).GetAwaiter().GetResult();
    }

    private void SetPhoneNumber(IEmployeeRepository employeeRepository, PhoneNumber phoneNumber)
    {
        SetPhoneNumberAsync(employeeRepository, phoneNumber).GetAwaiter().GetResult();
    }
}
