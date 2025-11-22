using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.DepartmentAggregate.Validators;

public class DepartmentNameValidator : AbstractValidator<string>
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentNameValidator(IDepartmentRepository departmentRepository, Guid departmentId)
    {
        _departmentRepository = departmentRepository;

        // Add validation rules here
        RuleFor(name => name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("The {PropertyName} is required.")
                .MinimumLength(2)
                .WithMessage("The {PropertyName} must be at least {MinLength} characters.")
                .MaximumLength(20)
                .WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
                .MustAsync(async (name, token) => await IsUniqueNameAsync(departmentId, name, token))
                .WithMessage("The DepartmentName is already existent.");
    }

    protected async Task<bool> IsUniqueNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        bool isUnique = await _departmentRepository.ExistsAsync(d => d.Name == name && d.Id != id, cancellationToken) == false;
        return isUnique;
    }
}
