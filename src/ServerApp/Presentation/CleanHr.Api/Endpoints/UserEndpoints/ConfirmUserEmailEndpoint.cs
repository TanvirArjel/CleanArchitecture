using System.ComponentModel.DataAnnotations;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.UserEndpoints;

[ApiVersion("1.0")]
public class ConfirmUserEmailEndpoint : UserEndpointBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public ConfirmUserEmailEndpoint(
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;
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
        bool isExistent = await _userManager.Users.Where(u => u.Email == model.Email).AnyAsync();

        if (isExistent == false)
        {
            ModelState.AddModelError(nameof(model.Email), "The provided email is not related to any account.");
            return ValidationProblem(ModelState);
        }

        GetEmailVerificationCodeQuery query = new GetEmailVerificationCodeQuery(model.Email, model.Code);

        EmailVerificationCode emailVerificationCode = await _mediator.Send(query);

        if (emailVerificationCode == null)
        {
            ModelState.AddModelError(string.Empty, "Either email or password reset code is incorrect.");
            return ValidationProblem(ModelState);
        }

        if (DateTime.UtcNow > emailVerificationCode.SentAtUtc.AddMinutes(5))
        {
            ModelState.AddModelError(nameof(model.Code), "The code is expired.");
            return ValidationProblem(ModelState);
        }

        VerifyUserEmailCommand verifyUserEmailCommand = new VerifyUserEmailCommand(model.Email, model.Code);

        await _mediator.Send(verifyUserEmailCommand);

        return Ok();
    }
}

public class EmailConfirmationModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6, ErrorMessage = "{0} should be {1} characters long.")]
    [MaxLength(6, ErrorMessage = "{0} should be {1} characters long.")]
    public string Code { get; set; }
}
