using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
internal class ConfirmUserEmailEndpoint(
    IMediator mediator) : UserEndpointBase
{
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
        await mediator.Send(verifyUserEmailCommand);
        return Ok();
    }
}
