using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Application.Infrastructures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
internal class ResetUserPasswordEndpoint(
    IMediator mediator,
    IExceptionLogger exceptionLogger) : UserEndpointBase
{
    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Reset a new password for an user by posting the password reset code and the new password.")]
    public async Task<IActionResult> Post(ResetPasswordModel model)
    {
        try
        {
            ResetPasswordCommand resetPasswordCommand = new(model.Email, model.Code, model.Password);
            await mediator.Send(resetPasswordCommand);
            return Ok();
        }
        catch (Exception exception)
        {
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;
            await exceptionLogger.LogAsync(exception, model);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
