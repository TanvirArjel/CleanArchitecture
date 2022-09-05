using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints.ExternalLoginEndpoints;

[Route("api/v{version:apiVersion}/external-login")]
[ApiExplorerSettings(GroupName = "External Login Endpoints")]
[ApiController]
public abstract class ExternalLoginEndpointBase : ControllerBase
{
}
