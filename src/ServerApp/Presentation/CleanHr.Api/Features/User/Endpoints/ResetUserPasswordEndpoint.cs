using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Application.Extensions;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class ResetUserPasswordEndpoint : UserEndpointBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ResetUserPasswordEndpoint> _logger;

    public ResetUserPasswordEndpoint(
        IMediator mediator,
        ILogger<ResetUserPasswordEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

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
            Result result = await _mediator.Send(resetPasswordCommand);

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
        catch (Exception exception)
        {
            model.Password = string.Empty;
            model.ConfirmPassword = string.Empty;


            Dictionary<string, object> fields = new()
            {
                { "RequestBody", model }
            };


            _logger.LogException(exception, "An error occurred during password reset.", fields);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
