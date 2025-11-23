using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.IdentityAggregate.Validators;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace CleanHr.Domain.Aggregates.IdentityAggregate;

public class ApplicationUser : IdentityUser<Guid>
{
    // Private constructor for new instances
    private ApplicationUser(Guid id)
    {
        Id = id;
        IsDisabled = false;
        EmailConfirmed = false;
        PhoneNumberConfirmed = false;
        TwoFactorEnabled = false;
        LockoutEnabled = true;
        AccessFailedCount = 0;
    }

    // This is needed for EF Core query mapping and Identity framework
    [JsonConstructor]
    public ApplicationUser()
    {
    }

    // Properties with private setters for encapsulation (internal for seeding)
    public string FullName { get; internal set; }

    public string DialCode { get; private set; }

    public string LanguageCulture { get; private set; }

    public bool IsDisabled { get; private set; }

    public DateTime? LastLoggedInAtUtc { get; private set; }

    // Navigation Properties
    public RefreshToken RefreshToken { get; private set; }

    /// <summary>
    /// Factory method for creating a new ApplicationUser with validation.
    /// </summary>
    /// <param name="repository">The instance of <see cref="IApplicationUserRepository"/>.</param>
    /// <param name="firstName">The first name of the user.</param>
    /// <param name="lastName">The last name of the user.</param>
    /// <param name="email">The email address.</param>
    /// <param name="password">The password.</param>
    /// <param name="userName">The username (optional, defaults to email if not provided).</param>
    /// <returns>Returns <see cref="Task{TResult}"/>.</returns>
    public static async Task<Result<ApplicationUser>> CreateAsync(
        IApplicationUserRepository repository,
        string firstName,
        string lastName,
        string email,
        string password,
        string userName = null)
    {
        ArgumentNullException.ThrowIfNull(repository);

        // Validate password
        PasswordValidator passwordValidator = new();
        ValidationResult passwordValidationResult = await passwordValidator.ValidateAsync(password);

        if (passwordValidationResult.IsValid == false)
        {
            return Result<ApplicationUser>.Failure(passwordValidationResult.ToDictionary());
        }

        // Validate FirstName and LastName components
        UserNameComponentValidator firstNameValidator = new("FirstName");
        ValidationResult firstNameResult = await firstNameValidator.ValidateAsync(firstName);

        if (firstNameResult.IsValid == false)
        {
            return Result<ApplicationUser>.Failure(firstNameResult.ToDictionary());
        }

        UserNameComponentValidator lastNameValidator = new("LastName");
        ValidationResult lastNameResult = await lastNameValidator.ValidateAsync(lastName);

        if (lastNameResult.IsValid == false)
        {
            return Result<ApplicationUser>.Failure(lastNameResult.ToDictionary());
        }

        ApplicationUserValidator validator = new();
        UniqueUserEmailValidator uniqueEmailValidator = new(repository);

        ApplicationUser user = new(Guid.NewGuid())
        {
            FullName = $"{firstName?.Trim()} {lastName?.Trim()}".Trim(),
            Email = email?.Trim(),
            UserName = userName?.Trim() ?? email?.Trim(),
            NormalizedEmail = email?.Trim().ToUpperInvariant(),
            NormalizedUserName = (userName?.Trim() ?? email?.Trim()).ToUpperInvariant()
        };

        ValidationResult validationResult = await validator.ValidateAsync(user);

        if (validationResult.IsValid == false)
        {
            return Result<ApplicationUser>.Failure(validationResult.ToDictionary());
        }

        ValidationResult uniqueEmailResult = await uniqueEmailValidator.ValidateAsync(user);

        if (uniqueEmailResult.IsValid == false)
        {
            return Result<ApplicationUser>.Failure(uniqueEmailResult.ToDictionary());
        }

        // If username is provided and different from email, validate uniqueness
        if (!string.IsNullOrWhiteSpace(userName) && userName != email)
        {
            UniqueUserNameValidator uniqueUserNameValidator = new(repository);
            ValidationResult uniqueUserNameResult = await uniqueUserNameValidator.ValidateAsync(user);

            if (uniqueUserNameResult.IsValid == false)
            {
                return Result<ApplicationUser>.Failure(uniqueUserNameResult.ToDictionary());
            }
        }

        return Result<ApplicationUser>.Success(user);
    }

    // Public methods for business logic
    public Result SetFullName(string firstName, string lastName)
    {
        // Validate FirstName and LastName components
        UserNameComponentValidator firstNameValidator = new("FirstName");
        ValidationResult firstNameResult = firstNameValidator.Validate(firstName);

        if (firstNameResult.IsValid == false)
        {
            return Result.Failure(firstNameResult.ToDictionary());
        }

        UserNameComponentValidator lastNameValidator = new("LastName");
        ValidationResult lastNameResult = lastNameValidator.Validate(lastName);

        if (lastNameResult.IsValid == false)
        {
            return Result.Failure(lastNameResult.ToDictionary());
        }

        string originalFullName = FullName;
        FullName = $"{firstName?.Trim()} {lastName?.Trim()}".Trim();

        ApplicationUserValidator validator = new();
        ValidationResult validationResult = validator.Validate(this);

        if (validationResult.IsValid == false)
        {
            FullName = originalFullName;
            return Result.Failure(validationResult.ToDictionary());
        }

        return Result.Success();
    }

    public async Task<Result> SetEmailAsync(IApplicationUserRepository repository, string email)
    {
        ArgumentNullException.ThrowIfNull(repository);

        string originalEmail = Email;
        string originalNormalizedEmail = NormalizedEmail;
        bool originalEmailConfirmed = EmailConfirmed;

        Email = email?.Trim();
        NormalizedEmail = email?.Trim().ToUpperInvariant();
        EmailConfirmed = false; // Reset email confirmation when email changes

        ApplicationUserValidator validator = new();
        UniqueUserEmailValidator uniqueEmailValidator = new(repository);

        ValidationResult validationResult = await validator.ValidateAsync(this);

        if (validationResult.IsValid == false)
        {
            Email = originalEmail;
            NormalizedEmail = originalNormalizedEmail;
            EmailConfirmed = originalEmailConfirmed;
            return Result.Failure(validationResult.ToDictionary());
        }

        ValidationResult uniqueEmailResult = await uniqueEmailValidator.ValidateAsync(this);

        if (uniqueEmailResult.IsValid == false)
        {
            Email = originalEmail;
            NormalizedEmail = originalNormalizedEmail;
            EmailConfirmed = originalEmailConfirmed;
            return Result.Failure(uniqueEmailResult.ToDictionary());
        }

        return Result.Success();
    }

    public Result SetDialCode(string dialCode)
    {
        DialCode = dialCode?.Trim();
        return Result.Success();
    }

    public Result SetLanguageCulture(string languageCulture)
    {
        LanguageCulture = languageCulture?.Trim();
        return Result.Success();
    }

    public Result Disable()
    {
        if (IsDisabled)
        {
            return Result.Failure("User is already disabled.");
        }

        IsDisabled = true;
        return Result.Success();
    }

    public Result Enable()
    {
        if (!IsDisabled)
        {
            return Result.Failure("User is already enabled.");
        }

        IsDisabled = false;
        return Result.Success();
    }

    public Result RecordLogin()
    {
        LastLoggedInAtUtc = DateTime.UtcNow;
        return Result.Success();
    }

    public async Task<Result> SetPasswordAsync(string password, IPasswordHasher<ApplicationUser> passwordHasher)
    {
        ArgumentNullException.ThrowIfNull(passwordHasher);

        // Validate password
        PasswordValidator passwordValidator = new();
        ValidationResult passwordValidationResult = await passwordValidator.ValidateAsync(password);

        if (passwordValidationResult.IsValid == false)
        {
            return Result.Failure(passwordValidationResult.ToDictionary());
        }

        // Hash and set the password
        string newHashedPassword = passwordHasher.HashPassword(this, password);
        PasswordHash = newHashedPassword;

        return Result.Success();
    }
}
