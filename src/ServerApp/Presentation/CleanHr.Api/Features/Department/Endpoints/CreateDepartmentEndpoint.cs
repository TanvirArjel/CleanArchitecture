using CleanHr.Api.Features.Department.Models;
using CleanHr.Application.Commands.DepartmentCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.Department.Endpoints;

public sealed class CreateDepartmentEndpoint : DepartmentEndpointBase
{
    private readonly IMediator _mediator;

    public CreateDepartmentEndpoint(
        IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create a new department by posting the required data.")]
    public async Task<ActionResult> Post(CreateDepartmentModel model)
    {
        CreateDepartmentCommand command = new(model.Name, model.Description);
        Result<Guid> result = await _mediator.Send(command, HttpContext.RequestAborted);

        if (result.IsSuccess == false)
        {
            foreach (KeyValuePair<string, string> error in result.Errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }

            return ValidationProblem(ModelState);
        }

        return Created($"/api/v1/departments/{result.Value}", model);
    }
}