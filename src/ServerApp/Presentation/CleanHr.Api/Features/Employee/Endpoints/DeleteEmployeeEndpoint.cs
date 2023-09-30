using CleanHr.Application.Commands.EmployeeCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public class DeleteEmployeeEndpoint(IMediator mediator) : EmployeeEndpointBase
{
    // DELETE: api/employees/5
    [HttpDelete("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Delete an existing employee by employee id.")]
    public async Task<ActionResult> Delete(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(employeeId), $"The value of {nameof(employeeId)} can't be empty.");
            return ValidationProblem(ModelState);
        }

        DeleteEmployeeCommand command = new DeleteEmployeeCommand(employeeId);

        await mediator.Send(command, HttpContext.RequestAborted);
        return NoContent();
    }
}
