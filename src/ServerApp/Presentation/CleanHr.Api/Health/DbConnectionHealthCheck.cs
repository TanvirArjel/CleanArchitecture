using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CleanHr.Api;

public class DbConnectionHealthCheck(
    string connectionString,
    ILogger<DbConnectionHealthCheck> logger) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("ConnectionString in DbHealthCheck is: {ConnectionString}", connectionString);
            using SqlConnection sqlConnection = new(connectionString);
            using SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = "SELECT 1";

            await sqlConnection.OpenAsync(cancellationToken);
            await sqlCommand.ExecuteScalarAsync(cancellationToken);
            await sqlConnection.CloseAsync();
            return HealthCheckResult.Healthy(description: "The database connection is fine.");
		}
		catch (Exception exception)
		{
            logger.LogCritical(exception: exception, "The exception happened with connection string : {ConnectionString}", connectionString);
            return HealthCheckResult.Unhealthy(description: exception.Message);
        }
    }
}
