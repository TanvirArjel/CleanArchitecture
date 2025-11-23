using CleanHr.Application.Commands.EmployeeCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public sealed class DeleteEmployeeEndpoint(IMediator mediator) : EmployeeEndpointBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

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

        DeleteEmployeeCommand command = new(employeeId);

        Result result = await _mediator.Send(command, HttpContext.RequestAborted);

        if (result.IsSuccess == false)
        {
            foreach (KeyValuePair<string, string> error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}
