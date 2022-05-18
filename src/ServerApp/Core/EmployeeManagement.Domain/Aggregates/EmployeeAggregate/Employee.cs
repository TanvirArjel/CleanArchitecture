using System;
using EmployeeManagement.Domain.Aggregates.DepartmentAggregate;
using EmployeeManagement.Domain.Entities;
using TanvirArjel.ArgumentChecker;

namespace EmployeeManagement.Domain.Aggregates.EmployeeAggregate;

public class Employee : BaseEntity
{
    internal Employee(
        string name,
        Guid departmentId,
        DateTime dateOfBirth,
        string email,
        string phoneNumber)
    {
        Id = Guid.NewGuid();
        SetName(name);
        SetDeparment(departmentId);
        SetDateOfBirth(dateOfBirth);
        SetEmail(email);
        SetPhoneNumber(phoneNumber);
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

    public Department Department { get; private set; }

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

    internal void SetDeparment(Guid departmentId)
    {
        DepartmentId = departmentId.ThrowIfEmpty(nameof(departmentId));
    }

    internal void SetEmail(string email)
    {
        Email = email.ThrowIfNullOrEmpty(nameof(email))
                     .ThrowIfNotValidEmail(nameof(email));
    }

    internal void SetPhoneNumber(string phoneNumber)
    {
        PhoneNumber = phoneNumber.ThrowIfNullOrEmpty(nameof(phoneNumber));
    }
}
