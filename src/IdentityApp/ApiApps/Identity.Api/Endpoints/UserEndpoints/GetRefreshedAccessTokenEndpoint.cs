using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Identity.Api.EndpointBases;
using Identity.Application.Services;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using TanvirArjel.ArgumentChecker;

namespace Identity.Api.Endpoints.UserEndpoints
{
    [ApiVersion("1.0")]
    public class GetRefreshedAccessTokenEndpoint : UserEndpoint
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IConfiguration _configuration;

        public GetRefreshedAccessTokenEndpoint(
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            IConfiguration configuration,
            ITokenGenerator tokenGenerator)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _configuration = configuration;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get a new access token for user by posting user's expired access token and refresh token.")]
        public async Task<IActionResult> Post(TokenRefreshModel model)
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
            LoginResponseModel jsonWebToken = await GenerateJsonWebToken(applicationUser);

            return Ok(jsonWebToken);
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

        private async Task<LoginResponseModel> GenerateJsonWebToken(ApplicationUser applicationUser)
        {
            applicationUser.ThrowIfNull(nameof(applicationUser));

            IList<string> roles = await _userManager.GetRolesAsync(applicationUser).ConfigureAwait(false);

            string accessToken = _tokenGenerator.GenerateJwtToken(applicationUser, roles);

            RefreshToken refreshToken = await _applicationUserService.GetRefreshTokenAsync(applicationUser.Id);

            if (refreshToken == null)
            {
                string token = _tokenGenerator.GenerateRefreshToken();
                refreshToken = await _applicationUserService.StoreRefreshTokenAsync(applicationUser.Id, token);
            }
            else
            {
                if (refreshToken.ExpireAtUtc < DateTime.UtcNow)
                {
                    string token = _tokenGenerator.GenerateRefreshToken();
                    refreshToken = await _applicationUserService.UpdateRefreshTokenAsync(applicationUser.Id, token);
                }
            }

            int tokenLifeTime = _configuration.GetValue<int>("Jwt:Lifetime"); // Seconds

            LoginResponseModel jsonWebToken = new LoginResponseModel()
            {
                UserId = applicationUser.Id,
                FullName = applicationUser.FullName,
                UserName = applicationUser.UserName,
                Email = applicationUser.Email,
                AccessToken = accessToken,
                AccessTokenExpireAtUtc = DateTime.UtcNow.AddSeconds(tokenLifeTime),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpireAtUtc = refreshToken.ExpireAtUtc,
            };

            return jsonWebToken;
        }
    }

    public class TokenRefreshModel
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
