using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Identity.Api.Helpers;
using Identity.Application.Infrastrucures;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Api.Endpoints.UserEndpoints
{
    [ApiVersion("1.0")]
    public class UserLoginEndpoint : UserEndpointBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TokenManager _tokenManager;
        private readonly IExceptionLogger _exceptionLogger;

        public UserLoginEndpoint(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IExceptionLogger exceptionLogger,
            TokenManager tokenManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _exceptionLogger = exceptionLogger;
            _tokenManager = tokenManager;
        }

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
                ApplicationUser applicationUser = await _userManager.FindByEmailAsync(loginModel.EmailOrUserName);

                if (applicationUser == null)
                {
                    ModelState.AddModelError(nameof(loginModel.EmailOrUserName), "The email does not exist.");
                    return BadRequest(ModelState);
                }

                Microsoft.AspNetCore.Identity.SignInResult signinResult = await _signInManager.PasswordSignInAsync(
                         loginModel.EmailOrUserName,
                         loginModel.Password,
                         isPersistent: loginModel.RememberMe,
                         lockoutOnFailure: false);

                if (signinResult.Succeeded)
                {
                    string jsonWebToken = await _tokenManager.GetJwtTokenAsync(applicationUser);
                    return Ok(jsonWebToken);
                }

                if (signinResult.IsNotAllowed)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(applicationUser))
                    {
                        ModelState.AddModelError(nameof(loginModel.EmailOrUserName), "The email is not confirmed yet.");
                        return BadRequest(ModelState);
                    }

                    if (!await _userManager.IsPhoneNumberConfirmedAsync(applicationUser))
                    {
                        ModelState.AddModelError(string.Empty, "The phone number is not confirmed yet.");
                        return BadRequest(ModelState);
                    }
                }
                else if (signinResult.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "The account is locked.");
                    return BadRequest(ModelState);
                }
                else if (signinResult.RequiresTwoFactor)
                {
                    ModelState.AddModelError(string.Empty, "Require two factor authentication.");
                    return BadRequest(ModelState);
                }
                else
                {
                    ModelState.AddModelError(nameof(loginModel.Password), "Password is incorrect.");
                    return BadRequest(ModelState);
                }

                return BadRequest(ModelState);
            }
            catch (Exception exception)
            {
                loginModel.Password = null;
                await _exceptionLogger.LogAsync(exception, loginModel);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "{0} should be between {2} to {1} characters")]
        public string EmailOrUserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
