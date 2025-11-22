using CleanHr.Api.Features.Department.Models;
using FluentValidation;
using MediatR;

namespace CleanHr.Api.Features.Department.Validators;

internal sealed class UpdateDepartmentModelValidator : AbstractValidator<UpdateDepartmentModel>
{
    public UpdateDepartmentModelValidator(IMediator mediator)
    {
        RuleFor(d => d.Id)
               .NotEmpty()
               .WithMessage("The Id is required.");
    }
}
