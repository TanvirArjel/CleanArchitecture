using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Identity.Api.ApiModels.IdentityModels;
using Identity.Application.Infrastrucures;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.ArgumentChecker;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Identity.Api.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user/[action]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IConfiguration _configuration;
        private readonly IExceptionLogger _exceptionLogger;

        public UserApiController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IApplicationUserService applicationUserService,
            IConfiguration configuration,
            IExceptionLogger exceptionLogger,
            ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationUserService = applicationUserService;
            _configuration = configuration;
            _exceptionLogger = exceptionLogger;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Create or register new user by posting the required data.")]
        public async Task<ActionResult> Register([FromBody] RegistrationModel registerModel)
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

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Resend email confiramtion code to the newly registered user's email.")]
        public async Task<ActionResult> ResendEmailConfirmationCode(ResendEmailConfirmationCodeModel model)
        {
            try
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

                bool isExists = await _applicationUserService.HasActiveEmailConfirmationCodeAsync(model.Email);

                if (isExists)
                {
                    ModelState.AddModelError(nameof(model.Email), "You already have an active code. Please wait! You may receive the code in your email. If not, please try again after sometimes.");
                    return BadRequest(ModelState);
                }

                await _applicationUserService.SendEmailVerificationCodeAsync(model.Email);
                return Ok();
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception, model);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Confirm the newly registered user's email by posting the required data.")]
        public async Task<ActionResult> ConfirmEmail(EmailConfirmationModel model)
        {
            try
            {
                IdentityError identityError = await _applicationUserService.VerifyEmailAsync(model.Email, model.Code);

                if (identityError != null)
                {
                    ModelState.AddModelError(identityError.Code, identityError.Description);
                    return BadRequest(ModelState);
                }

                ApplicationUser applicationUser = await _userManager.Users.Where(u => u.Email == model.Email).FirstOrDefaultAsync();
                return Ok();
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception, model);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Post the required credentials to get the access token for the login.")]
        public async Task<ActionResult<JsonWebTokenModel>> Login([FromBody] LoginModel loginModel)
        {
            try
            {
                ApplicationUser applicationUser = await _userManager.FindByEmailAsync(loginModel.EmailOrUserName);

                if (applicationUser == null)
                {
                    ModelState.AddModelError(nameof(loginModel.EmailOrUserName), "The email does not exist.");
                    return BadRequest(ModelState);
                }

                SignInResult signinResult = await _signInManager.PasswordSignInAsync(
                         loginModel.EmailOrUserName,
                         loginModel.Password,
                         isPersistent: loginModel.RememberMe,
                         lockoutOnFailure: false);

                if (signinResult.Succeeded)
                {
                    JsonWebTokenModel jsonWebToken = await GenerateJsonWebToken(applicationUser);
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

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get a new access token for user by posting user's expired access token and refresh token.")]
        public async Task<IActionResult> GetRefreshedAccessToken(TokenRefreshModel model)
        {
            try
            {
                ClaimsPrincipal claimsPrincipal;

                try
                {
                    claimsPrincipal = GetPrincipalFromExpiredToken(model.AccessToken);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(nameof(model.AccessToken), "Invalid access token.");
                    return BadRequest(ModelState);
                }

                string userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    ModelState.AddModelError(nameof(model.AccessToken), "Invalid access token.");
                    return BadRequest(ModelState);
                }

                bool isValid = await _applicationUserService.IsRefreshTokenValidAsync(Guid.Parse(userId), model.RefreshToken);

                if (!isValid)
                {
                    ModelState.AddModelError(nameof(model.RefreshToken), "Refresh token is not valid.");
                    return BadRequest(ModelState);
                }

                ApplicationUser applicationUser = await _userManager.FindByIdAsync(userId);
                JsonWebTokenModel jsonWebToken = await GenerateJsonWebToken(applicationUser);

                return Ok(jsonWebToken);
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception, model);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Send password reset code to reset user's password.")]
        public async Task<IActionResult> SendPasswordResetCode(ForgotPasswordModel forgotPasswordModel)
        {
            try
            {
                await _applicationUserService.SendPasswordResetCodeAsync(forgotPasswordModel.Email);
                return Ok();
            }
            catch (Exception exception)
            {
                await _exceptionLogger.LogAsync(exception, forgotPasswordModel);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Reset a new password for an user by posting the password reset code and the new password.")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                IdentityError identityError = await _applicationUserService.ResetPasswordAsync(model.Email, model.Code, model.Password);

                if (identityError != null)
                {
                    ModelState.AddModelError(identityError.Code, identityError.Description);
                    return BadRequest(ModelState);
                }

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

        private static string GenerateRefreshToken()
        {
            byte[] randomNumber = new byte[32];
            using RNGCryptoServiceProvider generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);

            string refreshToken = Convert.ToBase64String(randomNumber);

            return refreshToken;
        }

        private async Task<JsonWebTokenModel> GenerateJsonWebToken(ApplicationUser applicationUser)
        {
            applicationUser.ThrowIfNull(nameof(applicationUser));

            IList<string> roles = await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(false);

            string accessToken = _tokenGenerator.GenerateToken(applicationUser, roles);

            RefreshToken refreshToken = await _applicationUserService.GetRefreshTokenAsync(applicationUser.Id);

            if (refreshToken == null)
            {
                string token = GenerateRefreshToken();
                refreshToken = await _applicationUserService.StoreRefreshTokenAsync(applicationUser.Id, token);
            }
            else
            {
                if (refreshToken.ExpireAtUtc < DateTime.UtcNow)
                {
                    string token = GenerateRefreshToken();
                    refreshToken = await _applicationUserService.UpdateRefreshTokenAsync(applicationUser.Id, token);
                }
            }

            int tokenLifeTime = _configuration.GetValue<int>("Jwt:Lifetime"); // Seconds

            JsonWebTokenModel jsonWebToken = new JsonWebTokenModel()
            {
                UserId = applicationUser.Id,
                FullName = applicationUser.FirstName + " " + applicationUser.LastName,
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                AccessToken = accessToken,
                AccessTokenExpireAtUtc = DateTime.UtcNow.AddSeconds(tokenLifeTime),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireAtUtc = refreshToken.ExpireAtUtc,
            };

            return jsonWebToken;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Jwt:Key"))),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityTokenException("Invalid access token.");
            }

            return principal;
        }
    }
}
