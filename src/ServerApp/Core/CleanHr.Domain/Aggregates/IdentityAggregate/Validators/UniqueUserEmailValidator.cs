using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class UniqueUserEmailValidator : AbstractValidator<ApplicationUser>
{
    private readonly IApplicationUserRepository _userRepository;

    public UniqueUserEmailValidator(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<ApplicationUser> context, CancellationToken cancellation = default)
    {
        ApplicationUser user = context.InstanceToValidate;

        bool isEmailExistent = await _userRepository.ExistsAsync(u => u.Email == user.Email && u.Id != user.Id);

        if (isEmailExistent)
        {
            ValidationFailure validationFailure = new(nameof(user.Email), "A user already exists with the provided email.");
            return new ValidationResult(new[] { validationFailure });
        }

        return new ValidationResult();
    }
}
