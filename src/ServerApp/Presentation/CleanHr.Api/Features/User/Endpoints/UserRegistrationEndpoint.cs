using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Infrastructures;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class UserRegistrationEndpoint(
    UserManager<ApplicationUser> userManager,
    IExceptionLogger exceptionLogger) : UserEndpointBase
{
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
            ApplicationUser applicationUser = new()
            {
                FullName = model.FirstName + " " + model.LastName,
                UserName = model.Email,
                Email = model.Email
            };

            IdentityResult identityResult = await userManager.CreateAsync(applicationUser, model.Password);

            if (identityResult.Succeeded == false)
            {
                foreach (IdentityError item in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return ValidationProblem(ModelState);
            }

            //// await _applicationUserService.SendEmailVerificationCodeAsync(applicationUser.Email);
            return Ok();
        }
        catch (Exception exception)
        {
            model.Password = null;
            model.ConfirmPassword = null;
            await exceptionLogger.LogAsync(exception, model);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
