﻿using System;
using System.Text.Json;
using System.Threading.Tasks;
using CleanHr.Application.Infrastructures;
using Microsoft.Extensions.Logging;

namespace CleanHr.Infrastructure.Services;

internal sealed class ExceptionLogger(ILogger<ExceptionLogger> logger) : IExceptionLogger
{
    private readonly ILogger<ExceptionLogger> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task LogAsync(Exception exception)
    {
        await LogAsync(exception, null);
    }

    public async Task LogAsync(Exception exception, object parameters)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(exception);

            string jsonParameters = parameters != null ? JsonSerializer.Serialize(parameters) : "No parameter.";
            _logger.LogCritical(exception, "Parameters: {P1}", jsonParameters);

            await Task.CompletedTask;
        }
        catch (Exception loggerException)
        {
            _logger.LogCritical(loggerException, "Exception thrown in exception logger.");
        }
    }

    public async Task LogAsync(
        Exception exception,
        string requestPath,
        string requestBody)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(exception);

            _logger.LogCritical(exception, "RequestedPath: {P1} and RequestBody: {P2}", requestPath, requestBody);

            await Task.CompletedTask;
        }
        catch (Exception loggerException)
        {
            _logger.LogCritical(loggerException, "Exception thrown in exception logger.");
        }
    }
}
