using System;
using System.Threading.Tasks;
using EmployeeManagement.Application.Infrastrucures;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        private readonly IExceptionLogger _exceptionLogger;
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(IExceptionLogger exceptionLogger, ILogger<ErrorController> logger)
        {
            _exceptionLogger = exceptionLogger;
            _logger = logger;
        }

        [Route("/error")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Error()
        {
            try
            {
                IExceptionHandlerFeature context = HttpContext.Features.Get<IExceptionHandlerFeature>();
                await _exceptionLogger.LogAsync(context.Error);

                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, exception.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
