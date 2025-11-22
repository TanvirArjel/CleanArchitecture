using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.IdentityAggregate.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Domain.Aggregates.IdentityAggregate;

public class EmailVerificationCode
{
    private EmailVerificationCode(Guid id)
    {
        Id = id;
        SentAtUtc = DateTime.UtcNow;
    }

    // This is needed for EF Core query mapping
    [JsonConstructor]
    private EmailVerificationCode()
    {
    }

    public Guid Id { get; private set; }

    public string Email { get; private set; }

    public string Code { get; private set; }

    public DateTime SentAtUtc { get; private set; }

    public DateTime? UsedAtUtc { get; private set; }

    /// <summary>
    /// Factory method for creating a new EmailVerificationCode with validation.
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="email">The email address.</param>
    /// <param name="code">The verification code (6 digits).</param>
    /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
    public static async Task<Result<EmailVerificationCode>> CreateAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string code)
    {
        EmailVerificationCodeValidator validator = new(userManager);

        EmailVerificationCode verificationCode = new(Guid.NewGuid())
        {
            Email = email?.Trim(),
            Code = code?.Trim()
        };

        ValidationResult validationResult = await validator.ValidateAsync(verificationCode);

        if (validationResult.IsValid == false)
        {
            return Result<EmailVerificationCode>.Failure(validationResult.ToDictionary());
        }

        return Result<EmailVerificationCode>.Success(verificationCode);
    }

    public Result MarkAsUsed()
    {
        if (UsedAtUtc.HasValue)
        {
            return Result.Failure("The verification code has already been used.");
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
