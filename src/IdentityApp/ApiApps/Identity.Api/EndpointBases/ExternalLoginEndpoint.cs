using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.EndpointBases
{
    [Route("api/v{version:apiVersion}/external-login")]
    [ApiExplorerSettings(GroupName = "External Login Endpoints")]
    [ApiController]
    public abstract class ExternalLoginEndpoint : ControllerBase
    {
    }
}
