using System;
using System.Threading;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace CleanHr.Api;

public class DbConnectionHealthCheck : IHealthCheck
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<DbConnectionHealthCheck> _logger;

	public DbConnectionHealthCheck(
		IConfiguration configuration,
		ILogger<DbConnectionHealthCheck> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}

	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			bool isUnixLikeSystem = Environment.OSVersion.Platform == PlatformID.Unix
								|| Environment.OSVersion.Platform == PlatformID.MacOSX;

			string connectionString;

			if (isUnixLikeSystem)
			{
				connectionString = _configuration.GetConnectionString("DockerDbConnection");
			}
			else
			{
				connectionString = _configuration.GetConnectionString("EmployeeDbConnection");
			}

			_logger.LogInformation("ConnectionString in DbHealthCheck is: {ConnectionString}", connectionString);

			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			using SqlCommand sqlCommand = sqlConnection.CreateCommand();
			sqlCommand.CommandText = "SELECT 1";

			await sqlConnection.OpenAsync(cancellationToken);
			await sqlCommand.ExecuteScalarAsync(cancellationToken);
			await sqlConnection.CloseAsync();
			return HealthCheckResult.Healthy(description: "The the database connection is fine.");
		}
		catch (Exception exception)
		{
			_logger.LogCritical(exception.Message, exception);
			return HealthCheckResult.Unhealthy(description: exception.Message);
		}
	}
}
