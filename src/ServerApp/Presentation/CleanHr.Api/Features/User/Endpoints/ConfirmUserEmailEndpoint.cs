using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class ConfirmUserEmailEndpoint : UserEndpointBase
{
    private readonly IMediator _mediator;

    public ConfirmUserEmailEndpoint(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Confirm the newly registered user's email by posting the required data.")]
    public async Task<ActionResult> Post(EmailConfirmationModel model)
    {
        VerifyUserEmailCommand verifyUserEmailCommand = new(model.Email, model.Code);
        Result result = await _mediator.Send(verifyUserEmailCommand);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
}
