using System.Threading;
using Microsoft.Extensions.Logging;

namespace CleanHr.Api;

public class ConfigurationLoadingBackgroundService : BackgroundService
{
	private readonly ReadinessHealthCheck _readinessHealthCheck;
	private readonly ILogger<ConfigurationLoadingBackgroundService> _logger;

	public ConfigurationLoadingBackgroundService(
		ReadinessHealthCheck readinessHealthCheck,
		ILogger<ConfigurationLoadingBackgroundService> logger)
	{
		_readinessHealthCheck = readinessHealthCheck;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("Configuration loading has been started.");

		// Simulate the effect of a long-running task.
		await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

		_readinessHealthCheck.IsStartupCompleted = true;

		_logger.LogInformation("Configuration loading has been completed successfully.");
	}
}
