using System.Threading;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanHr.Api;

internal sealed class SendGridConnectionHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
