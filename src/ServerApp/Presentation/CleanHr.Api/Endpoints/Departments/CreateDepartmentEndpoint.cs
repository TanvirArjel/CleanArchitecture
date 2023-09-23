using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.Departments;

public class CreateDepartmentEndpoint(
    IMediator mediator) : DepartmentEndpointBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new department by posting the required data.")]
    public async Task<ActionResult> Post(CreateDepartmentModel model)
    {
        try
        {
            CreateDepartmentCommand command = new(model.Name, model.Description);

            Guid departmentId = await mediator.Send(command);
            return Created($"/api/v1/departments/{departmentId}", model);
        }
        catch (Exception exception)
        {
            if (exception is DomainValidationException)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return ValidationProblem(ModelState);
            }

            throw;
        }
    }
}

public class CreateDepartmentModel
{
    public string Name { get; set; }

    public string Description { get; set; }
}

public class CreateDepartmentModelValidator : AbstractValidator<CreateDepartmentModel>
{
    public CreateDepartmentModelValidator()
    {
        RuleFor(d => d.Name)
               .NotEmpty().WithMessage("The {PropertyName} is required.")
               .MaximumLength(20).WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
               .MinimumLength(2).WithMessage("The {PropertyName} must be at least {MinLength} characters.");

        RuleFor(d => d.Description)
               .NotEmpty().WithMessage("The {PropertyName} is required.")
               .MaximumLength(200).WithMessage("The {PropertyName} can't be more than {MaxLength} characters.")
               .MinimumLength(20).WithMessage("The {PropertyName} must be at least {MinLength} characters.");
    }
}