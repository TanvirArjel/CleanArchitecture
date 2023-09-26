using CleanHr.Api.Features.Department.Models;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class UpdateDepartmentEndpoint(
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
