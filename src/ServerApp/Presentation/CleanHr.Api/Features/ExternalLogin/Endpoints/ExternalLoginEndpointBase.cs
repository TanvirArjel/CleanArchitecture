using Microsoft.AspNetCore.Mvc;

namespace CleanHr.Api.Features.ExternalLogin.Endpoints;

[Route("api/v{version:apiVersion}/external-login")]
[ApiExplorerSettings(GroupName = "External Login Endpoints")]
[ApiController]
public abstract class ExternalLoginEndpointBase : ControllerBase
{
}
