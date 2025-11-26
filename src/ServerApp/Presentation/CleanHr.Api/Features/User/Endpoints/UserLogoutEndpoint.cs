using System.Security.Claims;
using CleanHr.Application.Commands.IdentityCommands.UserCommands;
using CleanHr.Application.Extensions;
using CleanHr.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class UserLogoutEndpoint : UserEndpointBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserLogoutEndpoint> _logger;

    public UserLogoutEndpoint(
        IMediator mediator,
        ILogger<UserLogoutEndpoint> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Authorize]
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesDefaultResponseType]
    [SwaggerOperation(Summary = "Logout the current user and revoke all their refresh tokens.")]
    public async Task<ActionResult> Post()
    {
        try
        {
            // Get the current user's ID from claims
            string userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid userId))
            {
                // Revoke all refresh tokens for this user
                RevokeAllUserRefreshTokensCommand command = new(userId);
                Result result = await _mediator.Send(command);

                if (result.IsSuccess == false)
                {
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning("Failed to revoke refresh tokens for user {UserId}", userId);
                    }
                }
            }

            return Ok(new { message = "Logged out successfully. All refresh tokens have been revoked." });
        }
        catch (Exception exception)
        {
            Dictionary<string, object> fields = new()
            {
                { "UserId", User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Unknown" }
            };
            _logger.LogException(exception, "An error occurred during logout", fields);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred during logout" });
        }
    }
}
