using CleanHr.Api.Features.Department.Models;
using FluentValidation;
using MediatR;

namespace CleanHr.Api.Features.Department.Validators;

public sealed class UpdateDepartmentModelValidator : DepartmentBaseModelValidator<UpdateDepartmentModel>
{
    public UpdateDepartmentModelValidator(IMediator mediator)
        : base(mediator)
    {
        RuleFor(d => d.Id)
               .NotEmpty()
               .WithMessage("The Id is required.");

        RuleFor(d => d.Name)
               .Cascade(CascadeMode.Stop)
               .ValidName()
               .MustAsync(async (model, name, token) => await IsUniqueNameAsync(model.Id, name, token))
               .WithMessage("The DepartmentName is already existent.");
    }
}
