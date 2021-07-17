using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.EndpointBases
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/departments")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Department Endpoints")]
    public class DepartmentEndpoint : ControllerBase
    {
    }
}
