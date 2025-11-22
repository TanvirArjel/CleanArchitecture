using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class SendUserPasswordResetCodeEndpoint(
    IMediator mediator) : UserEndpointBase
{
    [AllowAnonymous]
    [HttpPost("send-password-reset-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Send password reset code to reset user's password.")]
    public async Task<IActionResult> Post(ForgotPasswordModel model)
    {
        SendPasswordResetCodeCommand command = new(model.Email);
        Result result = await mediator.Send(command);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
}
