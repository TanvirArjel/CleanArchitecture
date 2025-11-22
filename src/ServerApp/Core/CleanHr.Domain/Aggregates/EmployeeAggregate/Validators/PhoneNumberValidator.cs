using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class PhoneNumberValidator : AbstractValidator<string>
{
    private readonly IEmployeeRepository _employeeRepository;

    public PhoneNumberValidator(IEmployeeRepository employeeRepository, Guid employeeId)
    {
        _employeeRepository = employeeRepository;

        // PhoneNumber validation
        RuleFor(phoneNumber => phoneNumber)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .MinimumLength(10)
            .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
            .MaximumLength(20)
            .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
            .MustAsync(async (phoneNumber, cancellationToken) =>
                await BeUniquePhoneNumberAsync(employeeId, phoneNumber, cancellationToken))
            .WithMessage("The phone number '{PropertyValue}' is already in use.");
    }

    public async Task<bool> BeUniquePhoneNumberAsync(Guid employeeId, string phoneNumber, CancellationToken cancellationToken = default)
    {
        bool isPhoneNumberExistent = await _employeeRepository.ExistsAsync(e => e.PhoneNumber == phoneNumber && e.Id != employeeId, cancellationToken);

        return !isPhoneNumberExistent;
    }
}
