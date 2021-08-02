using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Identity.Api.EndpointBases;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Api.Endpoints.UserEndpoints
{
    [ApiVersion("1.0")]
    public class SendUserPasswordResetCodeEndpoint : UserEndpoint
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;

        public SendUserPasswordResetCodeEndpoint(
            IApplicationUserService applicationUserService,
            UserManager<ApplicationUser> userManager)
        {
            _applicationUserService = applicationUserService;
            _userManager = userManager;
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
            }

            await _applicationUserService.SendPasswordResetCodeAsync(model.Email);
            return Ok();
        }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
