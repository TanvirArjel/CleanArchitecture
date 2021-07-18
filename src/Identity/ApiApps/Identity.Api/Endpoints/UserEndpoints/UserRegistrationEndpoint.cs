using System;
using System.Threading.Tasks;
using Identity.Api.ApiModels.IdentityModels;
using Identity.Api.EndpointBases;
using Identity.Application.Infrastrucures;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Api.Endpoints.UserEndpoints
{
    public class UserRegistrationEndpoint : UserEndpoint
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IExceptionLogger _exceptionLogger;

        public UserRegistrationEndpoint(UserManager<ApplicationUser> userManager, IExceptionLogger exceptionLogger)
        {
            _userManager = userManager;
            _exceptionLogger = exceptionLogger;
        }

        [HttpPost("registration")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Create or register new user by posting the required data.")]
        public async Task<ActionResult> Post(RegistrationModel registerModel)
        {
            try
            {
                ApplicationUser applicationUser = new ApplicationUser
                {
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    UserName = registerModel.Email,
                    Email = registerModel.Email
                };

                IdentityResult identityResult = await _userManager.CreateAsync(applicationUser, registerModel.Password);

                if (identityResult.Succeeded == false)
                {
                    foreach (IdentityError item in identityResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, item.Description);
                    }

                    return BadRequest(ModelState);
                }

                //// await _applicationUserService.SendEmailVerificationCodeAsync(applicationUser.Email);
                return Ok();
            }
            catch (Exception exception)
            {
                registerModel.Password = null;
                registerModel.ConfirmPassword = null;
                await _exceptionLogger.LogAsync(exception, registerModel);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
