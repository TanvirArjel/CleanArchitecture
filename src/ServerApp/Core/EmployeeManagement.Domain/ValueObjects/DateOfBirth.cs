using System;
using System.Collections.Generic;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

namespace EmployeeManagement.Domain.ValueObjects;

public sealed class DateOfBirth : ValueObject
{
    private readonly DateTime _minDateOfBirth = DateTime.UtcNow.AddYears(-115);

    private readonly DateTime _maxDateOfBirth = DateTime.UtcNow.AddYears(-15);

    public DateOfBirth(DateTime value)
    {
        SetValue(value);
    }

    public DateTime Value { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private void SetValue(DateTime value)
    {
        if (value < _minDateOfBirth || value > _maxDateOfBirth)
        {
            throw new DomainValidationException("The minimum age has to be 15 years.");
        }

        Value = value;
    }
}
