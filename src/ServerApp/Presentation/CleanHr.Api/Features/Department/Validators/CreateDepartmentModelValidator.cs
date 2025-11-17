using CleanHr.Api.Features.Department.Models;
using FluentValidation;
using MediatR;

namespace CleanHr.Api.Features.Department.Validators;

internal sealed class CreateDepartmentModelValidator : DepartmentBaseModelValidator<CreateDepartmentModel>
{
    public CreateDepartmentModelValidator(IMediator mediator)
        : base(mediator)
    {
        RuleFor(d => d.Name)
               .ValidName()
               .MustAsync(async (model, name, token) => await IsUniqueNameAsync(Guid.Empty, name, token))
               .WithMessage("The DepartmentName is already existent.");
    }
}