using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace CleanHr.Domain.Aggregates.IdentityAggregate.Validators;

public class UniqueUserNameValidator : AbstractValidator<ApplicationUser>
{
    private readonly IApplicationUserRepository _userRepository;

    public UniqueUserNameValidator(IApplicationUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<ApplicationUser> context, CancellationToken cancellation = default)
    {
        ApplicationUser user = context.InstanceToValidate;

        bool isUserNameExistent = await _userRepository.ExistsAsync(u => u.UserName == user.UserName && u.Id != user.Id);

        if (isUserNameExistent)
        {
            ValidationFailure validationFailure = new(nameof(user.UserName), "A user already exists with the provided username.");
            return new ValidationResult(new[] { validationFailure });
        }

        return new ValidationResult();
    }
}
