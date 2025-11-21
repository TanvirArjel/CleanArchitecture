using System.Threading;
using Microsoft.Extensions.Logging;

namespace CleanHr.Api;

internal sealed class ConfigurationLoadingBackgroundService(
	ReadinessHealthCheck readinessHealthCheck,
	ILogger<ConfigurationLoadingBackgroundService> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		logger.LogInformation("Configuration loading has been started.");

		// Simulate the effect of a long-running task.
		await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

		readinessHealthCheck.IsStartupCompleted = true;

		logger.LogInformation("Configuration loading has been completed successfully.");
	}
}
