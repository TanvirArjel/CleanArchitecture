using CleanHr.Api.Features.Department.Models;
using CleanHr.Application.Commands.DepartmentCommands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class CreateDepartmentEndpoint(
    IMediator mediator) : DepartmentEndpointBase
{
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new department by posting the required data.")]
    public async Task<ActionResult> Post(CreateDepartmentModel model)
    {
        CreateDepartmentCommand command = new(model.Name, model.Description);
        Guid departmentId = await _mediator.Send(command, HttpContext.RequestAborted);
        return Created($"/api/v1/departments/{departmentId}", model);
    }
}