using System.Collections.Generic;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

namespace EmployeeManagement.Domain.Aggregates.ValueObjects;
public sealed class Name : ValueObject
{
    public const int MinLength = 2;
    public const int MaxLength = 50;

    public Name(string firstName, string lastName)
    {
        SetFirstName(firstName);
        SetLastName(lastName);
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
    }

    private void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new DomainValidationException("The FirstName cannot be null or empty.");
        }

        if (firstName.Length < MinLength || firstName.Length > MaxLength)
        {
            throw new DomainValidationException($"The FirstName must be in between {MinLength} && {MaxLength} characters.");
        }

        FirstName = firstName;
    }

    private void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            throw new DomainValidationException("The LastName cannot be null or empty.");
        }

        if (lastName.Length < MinLength || lastName.Length > MaxLength)
        {
            throw new DomainValidationException($"The LastName must be in between {MinLength} && {MaxLength} characters.");
        }

        LastName = lastName;
    }
}
