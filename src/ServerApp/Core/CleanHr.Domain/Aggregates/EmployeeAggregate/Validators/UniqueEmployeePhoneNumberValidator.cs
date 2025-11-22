using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class UniqueEmployeePhoneNumberValidator : AbstractValidator<Employee>
{
    private readonly IEmployeeRepository _employeeRepository;

    public UniqueEmployeePhoneNumberValidator(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<Employee> context, CancellationToken cancellation = default)
    {
        Employee employee = context.InstanceToValidate;

        bool isPhoneNumberExistent = await _employeeRepository.ExistsAsync(e => e.PhoneNumber == employee.PhoneNumber && e.Id != employee.Id);

        if (isPhoneNumberExistent)
        {
            ValidationFailure validationFailure = new(nameof(employee.PhoneNumber), "An employee already exists with the provided phone number.");
            return new ValidationResult(new[] { validationFailure });
        }

        return new ValidationResult();
    }
}
