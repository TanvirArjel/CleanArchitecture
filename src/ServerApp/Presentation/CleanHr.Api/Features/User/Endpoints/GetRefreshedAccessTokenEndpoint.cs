using CleanHr.Api.Features.User.Models;
using CleanHr.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class GetRefreshedAccessTokenEndpoint(
    JwtTokenManager tokenManager) : UserEndpointBase
{
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Get a new access token for user by posting user's expired access token and refresh token.")]
    public async Task<ActionResult<string>> Post(TokenRefreshModel model)
    {
        string jsonWebToken = await tokenManager.GetTokenAsync(model.AccessToken, model.RefreshToken);
        return Ok(jsonWebToken);
    }
}
