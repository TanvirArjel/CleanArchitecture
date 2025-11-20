using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace CleanHr.Api.Features.User.Endpoints;

[ApiVersion("1.0")]
public class UserLogoutEndpoint : UserEndpointBase
{
    [HttpPost("logout")]
    public async Task<ActionResult> Post()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
}
