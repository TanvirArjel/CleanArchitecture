namespace CleanHr.Api;

internal static class ConfigurationHelper
{
	public static string GetDbConnectionString(this WebApplicationBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);

		bool isUnixLikeSystem = Environment.OSVersion.Platform == PlatformID.Unix
								|| Environment.OSVersion.Platform == PlatformID.MacOSX;

		string connectionString;

		if (isUnixLikeSystem)
		{
			connectionString = builder.Configuration.GetConnectionString("DockerDbConnection");
		}
		else
		{
			string connectionName = builder.Environment.IsDevelopment() ? "EmployeeDbConnection" : "EmployeeDbConnection";
			connectionString = builder.Configuration.GetConnectionString(connectionName);
		}

		return connectionString;
	}
}
