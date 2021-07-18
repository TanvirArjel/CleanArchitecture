using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.EndpointBases
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/user")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "User Endpoints")]
    public class UserEndpoint : ControllerBase
    {
    }
}
