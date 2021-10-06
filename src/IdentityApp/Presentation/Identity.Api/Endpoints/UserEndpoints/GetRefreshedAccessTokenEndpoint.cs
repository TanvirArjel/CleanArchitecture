﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Api.Helpers;
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
    public class GetRefreshedAccessTokenEndpoint : UserEndpointBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserService _applicationUserService;
        private readonly TokenManager _tokenManager;

        public GetRefreshedAccessTokenEndpoint(
            UserManager<ApplicationUser> userManager,
            IApplicationUserService applicationUserService,
            TokenManager tokenManager)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _tokenManager = tokenManager;
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesDefaultResponseType]
        [SwaggerOperation(Summary = "Get a new access token for user by posting user's expired access token and refresh token.")]
        public async Task<ActionResult<string>> Post(TokenRefreshModel model)
        {
            ClaimsPrincipal claimsPrincipal;

            try
            {
                claimsPrincipal = _tokenManager.ParseExpiredToken(model.AccessToken);
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
            string jsonWebToken = await _tokenManager.GetJwtTokenAsync(applicationUser);

            return Ok(jsonWebToken);
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