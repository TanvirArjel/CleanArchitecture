using System.ComponentModel.DataAnnotations;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.Departments;

public class UpdateDepartmentEndpoint(
    IMediator mediator) : DepartmentEndpointBase
{
    [HttpPut("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Update an existing employee by employee id and posting updated data.")]
    public async Task<ActionResult> Put(Guid departmentId, UpdateDepartmentModel model)
    {
        try
        {
            if (departmentId != model.Id)
            {
                ModelState.AddModelError(nameof(model.Id), "The DepartmentId does not match with route value.");
                return ValidationProblem(ModelState);
            }

            UpdateDepartmentCommand command = new UpdateDepartmentCommand(departmentId, model.Name, model.Description, true);

            await mediator.Send(command);
            return Ok();
        }
        catch (Exception exception)
        {
            if (exception is EntityNotFoundException)
            {
                ModelState.AddModelError(nameof(model.Id), "The Department does not exist.");
                return ValidationProblem(ModelState);
            }

            if (exception is DomainValidationException)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
                return ValidationProblem(ModelState);
            }

            throw;
        }
    }
}

public class UpdateDepartmentModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(20, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Name { get; set; }

    [MaxLength(200, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(20, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Description { get; set; }
}
