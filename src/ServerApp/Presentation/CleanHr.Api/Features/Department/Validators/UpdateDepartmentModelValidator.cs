using System;
using CleanHr.Api.Features.Department.Models;
using FluentValidation;

namespace CleanHr.Api.Features.Department.Validators;

public sealed class UpdateDepartmentModelValidator : DepartmentBaseModelValidator<UpdateDepartmentModel>
{
    public UpdateDepartmentModelValidator()
    {
        RuleFor(d => d.Id)
               .NotEmpty()
               .WithMessage("The Id is required.");
    }
}
