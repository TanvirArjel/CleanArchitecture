using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.Departments;

public class DeleteDepartmentEndpoint(
    IMediator mediator) : DepartmentEndpointBase
{
    [HttpDelete("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Delete an existing department by department id.")]
    public async Task<IActionResult> Delete(Guid departmentId)
    {
        try
        {
            if (departmentId == Guid.Empty)
            {
                ModelState.AddModelError(string.Empty, $"The value of {nameof(departmentId)} must be not empty.");
                return ValidationProblem(ModelState);
            }

            DeleteDepartmentCommand command = new DeleteDepartmentCommand(departmentId);
            await mediator.Send(command);

            return NoContent();
        }
        catch (Exception exception)
        {
            if (exception is EntityNotFoundException)
            {
                ModelState.AddModelError(nameof(departmentId), "The Department does not exist.");
                return ValidationProblem(ModelState);
            }

            throw;
        }
    }
}
