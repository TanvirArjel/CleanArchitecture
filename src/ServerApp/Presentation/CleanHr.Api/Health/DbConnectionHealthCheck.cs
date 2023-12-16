﻿using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CleanHr.Api;

public class DbConnectionHealthCheck(
    string connectionString,
    ILogger<DbConnectionHealthCheck> logger) : IHealthCheck
{
    private readonly string _connectionString = connectionString;
    private readonly ILogger<DbConnectionHealthCheck> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("ConnectionString in DbHealthCheck is: {ConnectionString}", _connectionString);
            using SqlConnection sqlConnection = new(_connectionString);
            using SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "SELECT 1";

            await sqlConnection.OpenAsync(cancellationToken);
            await sqlCommand.ExecuteScalarAsync(cancellationToken);
            await sqlConnection.CloseAsync();
            return HealthCheckResult.Healthy(description: "The database connection is fine.");
		}
		catch (Exception exception)
		{
            _logger.LogCritical(exception: exception, "The exception happened with connection string : {ConnectionString}", _connectionString);
            return HealthCheckResult.Unhealthy(description: exception.Message);
        }
    }
}
