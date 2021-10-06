using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Identity.Api.Endpoints.UserEndpoints
{
    [ApiVersion("1.0")]
    public class ResendUserEmailConfirmationCodeEndpoint : UserEndpointBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IEmailVerificationCodeService _emailVerificationCodeService;

        public ResendUserEmailConfirmationCodeEndpoint(
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            IEmailVerificationCodeService emailVerificationCodeService)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _emailVerificationCodeService = emailVerificationCodeService;
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
                return BadRequest(ModelState);
            }

            if (applicationUser.EmailConfirmed)
            {
                ModelState.AddModelError(nameof(model.Email), "Email is already confirmed.");
                return BadRequest(ModelState);
            }

            bool isExists = await _emailVerificationCodeService.HasActiveCodeAsync(model.Email);

            if (isExists)
            {
                ModelState.AddModelError(nameof(model.Email), "You already have an active code. Please wait! You may receive the code in your email. If not, please try again after sometimes.");
                return BadRequest(ModelState);
            }

            await _applicationUserService.SendEmailVerificationCodeAsync(model.Email);
            return Ok();
        }
    }

    public class ResendEmailConfirmationCodeModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
