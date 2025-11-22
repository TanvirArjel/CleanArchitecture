using System;
using System.Threading.Tasks;
using CleanHr.Domain.Aggregates.DepartmentAggregate;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.EmployeeAggregate.Validators;

public class EmployeeDepartmentValidator : AbstractValidator<Guid>
{
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeDepartmentValidator(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));

        // DepartmentId validation
        RuleFor(departmentId => departmentId)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("The {PropertyName} is required.");
    }

    public async Task<bool> IsValidDepartmentAsync(Guid departmentId)
    {
        // Check if department exists
        bool isDepartmentExistent = await _departmentRepository.ExistsAsync(d => d.Id == departmentId);
        return isDepartmentExistent;
    }
}
