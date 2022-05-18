using EmployeeManagement.Application.Commands.EmployeeCommands;
using EmployeeManagement.Application.Queries.EmployeeQueries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.Employees;

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
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Delete an existing employee by employee id.")]
    public async Task<ActionResult> Delete(Guid employeeId)
    {
        if (employeeId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(employeeId), $"The value of {nameof(employeeId)} can't be empty.");
            return BadRequest(ModelState);
        }

        IsEmployeeExistentByIdQuery existentByIdQuery = new IsEmployeeExistentByIdQuery(employeeId);

        bool isExistent = await _mediator.Send(existentByIdQuery);

        if (isExistent == false)
        {
            ModelState.AddModelError(nameof(employeeId), "The Employee does not exist.");
            return BadRequest(ModelState);
        }

        DeleteEmployeeCommand command = new DeleteEmployeeCommand(employeeId);

        await _mediator.Send(command);
        return NoContent();
    }
}
