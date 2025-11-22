using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class ResendUserEmailConfirmationCodeEndpoint(
    IMediator mediator) : UserEndpointBase
{
    [HttpPost("resend-email-confirmation-code")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Resend email confirmation code to the newly registered user's email.")]
    public async Task<ActionResult> Post(ResendEmailConfirmationCodeModel model)
    {
        SendEmailVerificationCodeCommand command = new(model.Email);
        Result result = await mediator.Send(command);

        if (result.IsSuccess == false)
        {
            return BadRequest(result.Errors);
        }

        return Ok();
    }
}
