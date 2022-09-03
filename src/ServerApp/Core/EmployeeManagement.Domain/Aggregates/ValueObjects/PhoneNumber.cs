using System.Collections.Generic;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

namespace EmployeeManagement.Domain.Aggregates.ValueObjects;

public sealed class PhoneNumber : ValueObject
{
    public const int MinLength = 10;
    public const int MaxLength = 20;

    public PhoneNumber(string value)
    {
        SetValue(value);
    }

    public string Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private void SetValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("The PhoneNumber value cannot be null or empty.");
        }

        if (value.Length < MinLength || value.Length > MaxLength)
        {
            throw new DomainValidationException($"The PhoneNumber value must be in between {MinLength} && {MaxLength} characters.");
        }

        Value = value;
    }
}
