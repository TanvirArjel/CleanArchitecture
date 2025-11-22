using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.IdentityAggregate.Validators;
using FluentValidation.Results;

namespace CleanHr.Domain.Aggregates.IdentityAggregate;

public class PasswordResetCode
{
    private PasswordResetCode(Guid id)
    {
        Id = id;
        SentAtUtc = DateTime.UtcNow;
    }

    // This is needed for EF Core query mapping
    [JsonConstructor]
    private PasswordResetCode()
    {
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string Code { get; private set; }

    public DateTime SentAtUtc { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    /// <summary>
    /// Factory method for creating a new PasswordResetCode with validation.
    /// </summary>
    /// <param name="email">The email address.</param>
    /// <param name="code">The reset code (6 digits).</param>
    /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
    public static async Task<Result<PasswordResetCode>> CreateAsync(string email, string code)
    {
        PasswordResetCodeValidator validator = new();

        PasswordResetCode resetCode = new(Guid.NewGuid())
        {
            Email = email?.Trim(),
            Code = code?.Trim()
        };

        ValidationResult validationResult = await validator.ValidateAsync(resetCode);

        if (validationResult.IsValid == false)
        {
            return Result<PasswordResetCode>.Failure(validationResult.ToDictionary());
        }

        return Result<PasswordResetCode>.Success(resetCode);
    }

    public Result MarkAsUsed()
    {
        if (UsedAtUtc.HasValue)
        {
            return Result.Failure("The password reset code has already been used.");
        }

        UsedAtUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public Result<bool> IsExpired(int expirationMinutes = 5)
    {
        bool isExpired = DateTime.UtcNow > SentAtUtc.AddMinutes(expirationMinutes);
        return Result<bool>.Success(isExpired);
    }
}
