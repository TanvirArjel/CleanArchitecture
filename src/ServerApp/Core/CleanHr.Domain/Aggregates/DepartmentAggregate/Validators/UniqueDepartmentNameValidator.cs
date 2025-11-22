using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;

namespace CleanHr.Domain.Aggregates.DepartmentAggregate.Validators;

public class UniqueDepartmentNameValidator : AbstractValidator<Department>
{
    private readonly IDepartmentRepository _departmentRepository;

    public UniqueDepartmentNameValidator(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;

        // Add validation rules here
        RuleFor(d => d.Name)
                .MustAsync(async (model, name, token) => await IsUniqueNameAsync(model.Id, name, token))
                .WithMessage("The DepartmentName is already existent.");
    }

    protected async Task<bool> IsUniqueNameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        bool isUnique = await _departmentRepository.ExistsAsync(d => d.Name == name && d.Id != id, cancellationToken) == false;
        return isUnique;
    }
}
