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

                if (paramters != null)
                {
                    string jsonParamters = JsonSerializer.Serialize(paramters);
                    _logger.LogCritical(exception, "Paramters: {JsonParamters}", jsonParamters);
                }
                else
                {
                    _logger.LogCritical(exception, "Exception Occured: {P1}", exception.Message);
                }

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

                _logger.LogCritical(exception, "RequestPath: {RequestPath} and RequestBody: {RequestBody}", requestPath, requestBody);

                await Task.CompletedTask;
            }
            catch (Exception loggerException)
            {
                _logger.LogCritical(loggerException, "Exception thrown in exception logger.");
            }
        }
    }
}
