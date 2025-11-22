using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class EmployeeEmailValidator : AbstractValidator<string>
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeEmailValidator(IEmployeeRepository employeeRepository, Guid employeeId)
    {
        _employeeRepository = employeeRepository;

        // Email validation
        RuleFor(email => email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.")
            .MaximumLength(50)
            .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
            .Must(BeValidEmail)
            .WithMessage("The {PropertyName} is not a valid email address.")
            .MustAsync(async (email, cancellationToken) => await BeUniqueEmailAsync(employeeId, email, cancellationToken))
            .WithMessage("The {PropertyName} is already in use.");
    }

    private bool BeValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        Regex emailRegex = new(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        return emailRegex.IsMatch(email);
    }

    public async Task<bool> BeUniqueEmailAsync(Guid employeeId, string email, CancellationToken cancellation = default)
    {
        bool isEmailExistent = await _employeeRepository.ExistsAsync(e => e.Email == email && e.Id != employeeId, cancellation);

        return !isEmailExistent;
    }
}
