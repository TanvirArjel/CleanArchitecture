using CleanHr.Application.Commands.EmployeeCommands;
using CleanHr.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public class DeleteEmployeeEndpoint : EmployeeEndpointBase
{
    private readonly IMediator _mediator;

    public DeleteEmployeeEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    // DELETE: api/employees/5
    [HttpDelete("{employeeId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Delete an existing employee by employee id.")]
    public async Task<ActionResult> Delete(Guid employeeId)
    {
        try
        {
            if (employeeId == Guid.Empty)
            {
                ModelState.AddModelError(nameof(employeeId), $"The value of {nameof(employeeId)} can't be empty.");
                return ValidationProblem(ModelState);
            }

            DeleteEmployeeCommand command = new DeleteEmployeeCommand(employeeId);

            await _mediator.Send(command, HttpContext.RequestAborted);
            return NoContent();
        }
        catch (Exception exception)
        {
            if (exception is EntityNotFoundException)
            {
                ModelState.AddModelError(nameof(employeeId), "The Employee does not exist.");
                return ValidationProblem(ModelState);
            }

            throw;
        }
    }
}
