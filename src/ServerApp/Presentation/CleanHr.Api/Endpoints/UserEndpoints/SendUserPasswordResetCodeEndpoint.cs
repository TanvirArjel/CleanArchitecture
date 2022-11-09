using System.ComponentModel.DataAnnotations;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.UserEndpoints;

[ApiVersion("1.0")]
public class SendUserPasswordResetCodeEndpoint : UserEndpointBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public SendUserPasswordResetCodeEndpoint(
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    [AllowAnonymous]
    [HttpPost("send-password-reset-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Send password reset code to reset user's password.")]
    public async Task<IActionResult> Post(ForgotPasswordModel model)
    {
        bool isExistent = await _userManager.Users.Where(u => u.Email == model.Email).AnyAsync();

        if (!isExistent)
        {
            ModelState.AddModelError(nameof(model.Email), "The email does not belong to any user.");
            return ValidationProblem(ModelState);
        }

        SendPasswordResetCodeCommand command = new SendPasswordResetCodeCommand(model.Email);
        await _mediator.Send(command);

        return Ok();
    }
}

public class ForgotPasswordModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
