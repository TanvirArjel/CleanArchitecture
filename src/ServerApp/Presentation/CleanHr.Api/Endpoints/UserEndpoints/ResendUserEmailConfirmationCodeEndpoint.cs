using System.ComponentModel.DataAnnotations;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Application.Queries.IdentityQueries.UserQueries;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Endpoints.UserEndpoints;

[ApiVersion("1.0")]
public class ResendUserEmailConfirmationCodeEndpoint : UserEndpointBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMediator _mediator;

    public ResendUserEmailConfirmationCodeEndpoint(
        UserManager<ApplicationUser> userManager,
        IMediator mediator)
    {
        _userManager = userManager;
        _mediator = mediator;
    }

    [HttpPost("resend-email-confirmation-code")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Resend email confiramtion code to the newly registered user's email.")]
    public async Task<ActionResult> Post(ResendEmailConfirmationCodeModel model)
    {
        ApplicationUser applicationUser = await _userManager.FindByEmailAsync(model.Email);

        if (applicationUser == null)
        {
            ModelState.AddModelError(nameof(model.Email), "Provided email is not related to any account.");
            return ValidationProblem(ModelState);
        }

        if (applicationUser.EmailConfirmed)
        {
            ModelState.AddModelError(nameof(model.Email), "Email is already confirmed.");
            return ValidationProblem(ModelState);
        }

        HasUserActiveEmailVerificationCodeQuery query = new HasUserActiveEmailVerificationCodeQuery(model.Email);

        bool isExists = await _mediator.Send(query);

        if (isExists)
        {
            ModelState.AddModelError(nameof(model.Email), "You already have an active code. Please wait! You may receive the code in your email. If not, please try again after sometimes.");
            return ValidationProblem(ModelState);
        }

        SendEmailVerificationCodeCommand command = new SendEmailVerificationCodeCommand(model.Email);
        await _mediator.Send(command);

        return Ok();
    }
}

public class ResendEmailConfirmationCodeModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
