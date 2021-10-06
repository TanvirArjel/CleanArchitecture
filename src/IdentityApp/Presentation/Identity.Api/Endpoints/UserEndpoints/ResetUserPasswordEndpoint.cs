using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Identity.Application.Infrastrucures;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Api.Endpoints.UserEndpoints
{
    [ApiVersion("1.0")]
    public class ResetUserPasswordEndpoint : UserEndpointBase
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

    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "{0} should be exactly {1} characters long.")]
        public string Code { get; set; }
    }
}
