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
public class UserRegistrationEndpoint : UserEndpointBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegistrationEndpoint> _logger;

    public UserRegistrationEndpoint(
        IMediator mediator,
        ILogger<UserRegistrationEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("registration")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Create or register new user by posting the required data.")]
    public async Task<ActionResult> Post(RegistrationModel model)
    {
        try
        {
            RegisterUserCommand command = new(
                model.FirstName,
                model.LastName,
                model.Email,
                model.Password);

            Result<Guid> result = await _mediator.Send(command);

            if (result.IsSuccess == false)
            {
                foreach (KeyValuePair<string, string> error in result.Errors)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                return ValidationProblem(ModelState);
            }

            //// await _applicationUserService.SendEmailVerificationCodeAsync(model.Email);
            return Ok();
        }
        catch (Exception exception)
        {
            model.Password = null;
            model.ConfirmPassword = null;

            Dictionary<string, object> fields = new()
            {
                { "RequestBody", model }
            };

            _logger.LogException(exception, "An error occurred during user registration.", fields);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
