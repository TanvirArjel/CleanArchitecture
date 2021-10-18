using System;
using System.Text.Json;
using System.Threading.Tasks;
using Identity.Application.Infrastrucures;
using Microsoft.Extensions.Logging;
using TanvirArjel.ArgumentChecker;

namespace Identity.Infrastructure.Services
{
    internal class ExceptionLogger : IExceptionLogger
    {
        private readonly ILogger<ExceptionLogger> _logger;

        public ExceptionLogger(ILogger<ExceptionLogger> logger)
        {
            _logger = logger;
        }

        public async Task LogAsync(Exception exception)
        {
            await LogAsync(exception, null);
        }

        public async Task LogAsync(Exception exception, object paramters)
        {
            try
            {
                exception.ThrowIfNull(nameof(exception));

                string jsonParamters = paramters != null ? JsonSerializer.Serialize(paramters) : "No paratemr";
                _logger.LogCritical(exception, "Paramters: {P1}", jsonParamters);

                await Task.CompletedTask;
            }
            catch (Exception loggerException)
            {
                _logger.LogCritical(loggerException, "Exception thrown in exception logger.");
            }
        }

        public async Task LogAsync(Exception exception, string requestPath, string requestBody)
        {
            try
            {
                exception.ThrowIfNull(nameof(exception));

                _logger.LogCritical(exception, "RequestPath: {P1} and RequestBody: {P2}", requestPath, requestBody);

                await Task.CompletedTask;
            }
            catch (Exception loggerException)
            {
                _logger.LogCritical(loggerException, "Exception thrown in exception logger.");
            }
        }
    }
}
