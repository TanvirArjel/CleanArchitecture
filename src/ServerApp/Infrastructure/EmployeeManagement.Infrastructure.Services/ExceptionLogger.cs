using System;
using System.Text.Json;
using System.Threading.Tasks;
using EmployeeManagement.Application.Infrastructures;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Infrastructure.Services;

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

    public async Task LogAsync(Exception exception, object parameters)
    {
        try
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            string jsonParameters = parameters != null ? JsonSerializer.Serialize(parameters) : "No parameter.";
            _logger.LogCritical(exception, "Parameters: {P1}", jsonParameters);

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
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            _logger.LogCritical(exception, "RequestedPath: {P1} and RequestBody: {P2}", requestPath, requestBody);

            await Task.CompletedTask;
        }
        catch (Exception loggerException)
        {
            _logger.LogCritical(loggerException, "Exception thrown in exception logger.");
        }
    }
}
