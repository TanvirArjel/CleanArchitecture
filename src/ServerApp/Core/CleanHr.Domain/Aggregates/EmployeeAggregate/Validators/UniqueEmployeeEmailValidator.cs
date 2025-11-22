using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class UniqueEmployeeEmailValidator : AbstractValidator<Employee>
{
    private readonly IEmployeeRepository _employeeRepository;

    public UniqueEmployeeEmailValidator(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public override async Task<ValidationResult> ValidateAsync(ValidationContext<Employee> context, CancellationToken cancellation = default)
    {
        Employee employee = context.InstanceToValidate;

        bool isEmailExistent = await _employeeRepository.ExistsAsync(e => e.Email == employee.Email && e.Id != employee.Id);

        if (isEmailExistent)
        {
            ValidationFailure validationFailure = new(nameof(employee.Email), "An employee already exists with the provided email.");
            return new ValidationResult(new[] { validationFailure });
        }

        return new ValidationResult();
    }
}
