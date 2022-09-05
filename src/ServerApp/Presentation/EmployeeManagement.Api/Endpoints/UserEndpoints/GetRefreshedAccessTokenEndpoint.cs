using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EmployeeManagement.Api.Helpers;
using EmployeeManagement.Application.Queries.IdentityQueries.UserQueries;
using EmployeeManagement.Domain.Aggregates.IdentityAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace EmployeeManagement.Api.Endpoints.UserEndpoints;

[ApiVersion("1.0")]
public class GetRefreshedAccessTokenEndpoint : UserEndpointBase
{
    private readonly UserManager<User> _userManager;
    private readonly TokenManager _tokenManager;
    private readonly IMediator _mediator;

    public GetRefreshedAccessTokenEndpoint(
        UserManager<User> userManager,
        TokenManager tokenManager,
        IMediator mediator)
    {
        _userManager = userManager;
        _tokenManager = tokenManager;
        _mediator = mediator;
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

        IsRefreshTokenValidQuery isRefreshTokenValidQuery = new IsRefreshTokenValidQuery(Guid.Parse(userId), model.RefreshToken);

        bool isValid = await _mediator.Send(isRefreshTokenValidQuery);

        if (!isValid)
        {
            ModelState.AddModelError(nameof(model.RefreshToken), "Refresh token is not valid.");
            return BadRequest(ModelState);
        }

        User applicationUser = await _userManager.FindByIdAsync(userId);
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
