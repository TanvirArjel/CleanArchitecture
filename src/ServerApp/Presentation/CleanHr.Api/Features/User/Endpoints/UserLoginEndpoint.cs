using CleanHr.Api.Features.User.Models;
using CleanHr.Api.Helpers;
using CleanHr.Application.Infrastructures;
using CleanHr.Domain.Aggregates.IdentityAggregate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
internal class UserLoginEndpoint(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IExceptionLogger exceptionLogger,
    TokenManager tokenManager) : UserEndpointBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Post the required credentials to get the access token for the login.")]
    public async Task<ActionResult<string>> Post(LoginModel loginModel)
    {
        try
        {
            ApplicationUser applicationUser = await userManager.FindByEmailAsync(loginModel.EmailOrUserName);

            if (applicationUser == null)
            {
                ModelState.AddModelError(nameof(loginModel.EmailOrUserName), "The email does not exist.");
                return ValidationProblem(ModelState);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await signInManager.PasswordSignInAsync(
                     loginModel.EmailOrUserName,
                     loginModel.Password,
                     isPersistent: loginModel.RememberMe,
                     lockoutOnFailure: false);

            if (signInResult.Succeeded)
            {
                string jsonWebToken = await tokenManager.GetJwtTokenAsync(applicationUser);
                return Ok(jsonWebToken);
            }

            if (signInResult.IsNotAllowed)
            {
                if (!await userManager.IsEmailConfirmedAsync(applicationUser))
                {
                    ModelState.AddModelError(nameof(loginModel.EmailOrUserName), "The email is not confirmed yet.");
                    return ValidationProblem(ModelState);
                }

                if (!await userManager.IsPhoneNumberConfirmedAsync(applicationUser))
                {
                    ModelState.AddModelError(string.Empty, "The phone number is not confirmed yet.");
                    return ValidationProblem(ModelState);
                }
            }
            else if (signInResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "The account is locked.");
                return ValidationProblem(ModelState);
            }
            else if (signInResult.RequiresTwoFactor)
            {
                ModelState.AddModelError(string.Empty, "Require two factor authentication.");
                return ValidationProblem(ModelState);
            }
            else
            {
                ModelState.AddModelError(nameof(loginModel.Password), "Password is incorrect.");
                return ValidationProblem(ModelState);
            }

            return ValidationProblem(ModelState);
        }
        catch (Exception exception)
        {
            loginModel.Password = null;
            await exceptionLogger.LogAsync(exception, loginModel);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
