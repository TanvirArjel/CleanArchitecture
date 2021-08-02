using System.Threading.Tasks;
using Identity.Api.EndpointBases;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Endpoints.UserEndpoints
{
    [ApiVersion("1.0")]
    public class UserLogoutEndpoint : UserEndpoint
    {
        [HttpPost("logout")]
        public async Task<ActionResult> Post()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }
    }
}
