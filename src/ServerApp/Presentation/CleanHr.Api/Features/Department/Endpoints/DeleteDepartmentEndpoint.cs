using CleanHr.Application.Commands.DepartmentCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class DeleteDepartmentEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public DeleteDepartmentEndpoint(
        IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpDelete("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Delete an existing department by department id.")]
    public async Task<IActionResult> Delete(Guid departmentId)
    {
        if (departmentId == Guid.Empty)
        {
            ModelState.AddModelError(string.Empty, $"The value of {nameof(departmentId)} must be not empty.");
            return ValidationProblem(ModelState);
        }

        DeleteDepartmentCommand command = new(departmentId);
        await _mediator.Send(command, HttpContext.RequestAborted);

        return NoContent();
    }
}
