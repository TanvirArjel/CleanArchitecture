using CleanHr.Api.Features.Employee.Models;
using CleanHr.Application.Commands.EmployeeCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Employee.Endpoints;

public sealed class CreateEmployeeEndpoint(IMediator mediator) : EmployeeEndpointBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    // POST: api/employees
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new employee by posting the required data.")]
    public async Task<ActionResult> Post([FromBody] CreateEmployeeModel model)
    {
        CreateEmployeeCommand createEmployeeCommand = new(
                model.FirstName,
                model.LastName,
                model.DepartmentId,
                model.DateOfBirth,
                model.Email,
                model.PhoneNumber);

        Result<Guid> result = await _mediator.Send(createEmployeeCommand, HttpContext.RequestAborted);

        if (result.IsSuccess == false)
        {
            foreach (KeyValuePair<string, string> error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return ValidationProblem(ModelState);
        }

        return Created($"/api/v1/employees/{result.Value}", model);
    }
}
