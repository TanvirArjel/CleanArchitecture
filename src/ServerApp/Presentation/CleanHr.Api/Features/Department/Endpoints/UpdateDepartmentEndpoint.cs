using CleanHr.Api.Features.Department.Models;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class UpdateDepartmentEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public UpdateDepartmentEndpoint(
        IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPut("{departmentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Update an existing employee by employee id and posting updated data.")]
    public async Task<ActionResult> Put(Guid departmentId, UpdateDepartmentModel model)
    {
        if (departmentId != model.Id)
        {
            ModelState.AddModelError(nameof(model.Id), "The DepartmentId does not match with route value.");
            return ValidationProblem(ModelState);
        }

        UpdateDepartmentCommand command = new(departmentId, model.Name, model.Description, true);

        Result result = await _mediator.Send(command, HttpContext.RequestAborted);

        if (result.IsSuccess == false)
        {
            foreach (KeyValuePair<string, string> error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return ValidationProblem(ModelState);
        }

        return Ok();
    }
}
