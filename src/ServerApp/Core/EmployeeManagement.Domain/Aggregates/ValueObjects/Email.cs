using System.Collections.Generic;
using System.Text.RegularExpressions;
using EmployeeManagement.Domain.Exceptions;
using EmployeeManagement.Domain.Primitives;

namespace EmployeeManagement.Domain.Aggregates.ValueObjects;

public sealed class Email : ValueObject
{
    public Email(string value)
    {
        SetValue(value);
    }

    public string Value { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    private void SetValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainValidationException("The Email value cannot be null or empty.");
        }

        Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        Match match = emailRegex.Match(value);

        if (match.Success == false)
        {
            throw new DomainValidationException("The Email value is not a valid email.");
        }

        Value = value;
    }
}
