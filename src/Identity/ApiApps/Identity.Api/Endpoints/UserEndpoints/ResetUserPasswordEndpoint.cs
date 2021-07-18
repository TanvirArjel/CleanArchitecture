using System;
using System.Threading.Tasks;
using Identity.Api.ApiModels.IdentityModels;
using Identity.Api.EndpointBases;
using Identity.Application.Infrastrucures;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Api.Endpoints.UserEndpoints
{
    public class ResetUserPasswordEndpoint : UserEndpoint
    {
        private readonly IPasswordResetCodeService _passwordResetCodeService;
        private readonly IApplicationUserService _applicationUserService;

        private readonly IExceptionLogger _exceptionLogger;

        public ResetUserPasswordEndpoint(
            IApplicationUserService applicationUserService,
            IExceptionLogger exceptionLogger,
            IPasswordResetCodeService passwordResetCodeService)
        {
            _applicationUserService = applicationUserService;
            _exceptionLogger = exceptionLogger;
            _passwordResetCodeService = passwordResetCodeService;
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
                PasswordResetCode passwordResetCode = await _passwordResetCodeService.GetAsync(model.Email, model.Code);

                if (passwordResetCode == null)
                {
                    ModelState.AddModelError(string.Empty, "Either email or password reset code is incorrect");
                    return BadRequest(ModelState);
                }

                if (DateTime.UtcNow > passwordResetCode.SentAtUtc.AddMinutes(5))
                {
                    ModelState.AddModelError(nameof(model.Code), "The code is expired.");
                    return BadRequest(ModelState);
                }

                await _applicationUserService.ResetPasswordAsync(model.Email, model.Code, model.Password);

                return Ok();
            }
            catch (Exception exception)
            {
                model.Password = string.Empty;
                model.ConfirmPassword = string.Empty;
                await _exceptionLogger.LogAsync(exception, model);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
