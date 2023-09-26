using CleanHr.Api.Features.Department.Models;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class CreateDepartmentEndpoint(
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