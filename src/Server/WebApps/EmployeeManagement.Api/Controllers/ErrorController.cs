using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using EmployeeManagement.Application.Infrastrucures;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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

        /// <summary>
        /// This method is for global exception handling and logging during any http request.
        /// </summary>
        /// <returns>Returns 500 status code.</returns>
        [Route("/error")]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Error()
        {
            try
            {
                IExceptionHandlerFeature exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

                IHttpRequestFeature httpRequestFeature = HttpContext.Features.Get<IHttpRequestFeature>();
                string requestePath = httpRequestFeature.RawTarget;

                string requestBoy = string.Empty;

                try
                {
                    httpRequestFeature.Body.Seek(0, SeekOrigin.Begin);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Can't rewind body stream. " + ex.Message);
                }

                using (StreamReader reader = new StreamReader(httpRequestFeature.Body, Encoding.UTF8))
                {
                    requestBoy = await reader.ReadToEndAsync();
                }

                await _exceptionLogger.LogAsync(exceptionFeature.Error, requestePath, requestBoy);

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
