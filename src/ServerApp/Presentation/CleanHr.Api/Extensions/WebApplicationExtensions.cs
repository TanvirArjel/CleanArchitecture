using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace CleanHr.Api;

internal static class WebApplicationExtensions
{
	public static void AddHealthCheckEndpoints(this WebApplication app)
	{
		ArgumentNullException.ThrowIfNull(app, nameof(app));

		app.MapHealthChecks("/healthz", new HealthCheckOptions()
		{
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		app.MapHealthChecks("/healthz/database", new HealthCheckOptions()
		{
			Predicate = hc => hc.Tags.Contains("database"),
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		app.MapHealthChecks("/healthz/ready", new HealthCheckOptions()
		{
			Predicate = hc => hc.Tags.Contains("ready"),
			ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
		});

		app.MapHealthChecks("/healthz/live", new HealthCheckOptions
		{
			Predicate = _ => false
		});

		app.MapHealthChecksUI();
	}
}
