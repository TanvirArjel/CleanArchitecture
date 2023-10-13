using System.Collections.Generic;
using System.Text.RegularExpressions;
using CleanHr.Domain.Exceptions;
using CleanHr.Domain.Primitives;

namespace CleanHr.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private const int _maxLength = 50;

    public Email(string value)
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
            throw new DomainValidationException("The Email cannot be null or empty.");
        }

        if (value.Length > _maxLength)
        {
            throw new DomainValidationException($"The Email length must be less than {_maxLength + 1} characters.");
        }

        Regex emailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

        Match match = emailRegex.Match(value);

        if (match.Success == false)
        {
            throw new DomainValidationException("The Email value is not a valid email.");
        }

        Value = value;
    }
}
