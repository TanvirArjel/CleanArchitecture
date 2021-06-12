using System;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeManagement.Application.Infrastrucures;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Infrastructure.Services
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
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            if (paramters != null)
            {
                string jsonParamters = JsonSerializer.Serialize(paramters);
                _logger.LogCritical(exception, "Paramters: {0}", jsonParamters);
            }
            else
            {
                _logger.LogCritical(exception, exception.Message);
            }

            await Task.CompletedTask;
        }

        public async Task LogAsync(Exception exception, string requestPath, string requestBody)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            _logger.LogCritical(exception, "RequestPath: {0} and RequestBody: {1}", requestPath, requestBody);

            await Task.CompletedTask;
        }
    }
}
