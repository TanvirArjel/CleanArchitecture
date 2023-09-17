using System.ComponentModel.DataAnnotations;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Exceptions;
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
            CreateDepartmentCommand command = new CreateDepartmentCommand(model.Name, model.Description);

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
    [Required]
    [MaxLength(20, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Name { get; set; }

    [MaxLength(200, ErrorMessage = "The {0} can't be more than {1} characters.")]
    [MinLength(20, ErrorMessage = "The {0} must be at least {1} characters.")]
    public string Description { get; set; }
}