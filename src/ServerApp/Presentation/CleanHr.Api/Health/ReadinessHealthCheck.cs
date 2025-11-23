using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanHr.Api;

internal sealed class ReadinessHealthCheck : IHealthCheck
{
	public bool IsStartupCompleted { get; set; }

	public Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		if (IsStartupCompleted)
		{
			return Task.FromResult(HealthCheckResult.Healthy(description: "The startup task has completed."));
		}

		return Task.FromResult(HealthCheckResult.Unhealthy(description: "The startup task is still running."));
	}
}
