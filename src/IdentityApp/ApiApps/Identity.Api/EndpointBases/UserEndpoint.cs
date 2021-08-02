using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.EndpointBases
{
    [Authorize]
    [Route("api/v{version:apiVersion}/user")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "User Endpoints")]
    public abstract class UserEndpoint : ControllerBase
    {
    }
}
