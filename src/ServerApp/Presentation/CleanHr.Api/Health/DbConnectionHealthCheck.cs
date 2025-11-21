using System.Threading;
using CleanHr.Application.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CleanHr.Api;

internal sealed class DbConnectionHealthCheck : IHealthCheck
{
    private readonly string _connectionString;
    private readonly ILogger<DbConnectionHealthCheck> _logger;

    public DbConnectionHealthCheck(
        string connectionString,
        ILogger<DbConnectionHealthCheck> logger)
    {
        _connectionString = connectionString;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        Dictionary<string, object> fields = new()
        {
            { "ConnectionString", _connectionString }
        };

        try
        {
            _logger.LogWithLevel(LogLevel.Information, "Testing database connection...", fields);
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
            _logger.LogException(exception, "Database connection is unhealthy.", fields);
            return HealthCheckResult.Unhealthy(description: exception.Message);
        }
    }
}
