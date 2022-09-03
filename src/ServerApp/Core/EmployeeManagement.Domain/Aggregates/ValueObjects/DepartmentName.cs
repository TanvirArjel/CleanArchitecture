using System.Collections.Generic;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

namespace EmployeeManagement.Domain.Aggregates.ValueObjects;

public sealed class DepartmentName : ValueObject
{
    public const int MinLength = 5;
    public const int MaxLength = 50;

    public DepartmentName(string value)
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
            throw new DomainValidationException("The DepartmentName value cannot be null or empty.");
        }

        if (value.Length < MinLength || value.Length > MaxLength)
        {
            throw new DomainValidationException($"The DepartmentName value must be in between {MinLength} && {MaxLength} characters.");
        }

        Value = value;
    }
}
