# 📝 Serilog with Grafana Loki Setup Guide

This guide explains the structured logging implementation using Serilog and Grafana Loki.

## 🎯 Overview

The application now uses **Serilog** for structured logging with the following features:
- **Console output** for development debugging
- **Grafana Loki** for centralized log aggregation and searching
- **Automatic enrichment** with context (environment, machine, thread)
- **Integration with OpenTelemetry** for correlating logs with traces

## 📦 Installed Packages

The following NuGet packages were added:
- `Serilog.AspNetCore` (9.0.0) - ASP.NET Core integration
- `Serilog.Sinks.Grafana.Loki` (8.3.1) - Loki sink for log shipping
- `Serilog.Enrichers.Environment` (3.0.1) - Environment name enrichment
- `Serilog.Enrichers.Thread` (4.0.0) - Thread ID enrichment

## ⚙️ Configuration

### Program.cs

Serilog is configured in `Program.cs` with:

```csharp
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Application", "CleanHrApi")
    .WriteTo.Console(
        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.GrafanaLoki(
        "http://localhost:3100",
        labels: new List<LokiLabel>
        {
            new LokiLabel { Key = "app", Value = "cleanhr-api" },
            new LokiLabel { Key = "environment", Value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production" }
        })
    .CreateLogger();
```

### appsettings.json

Serilog configuration in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.Hosting.Lifetime": "Information",
        "Microsoft.EntityFrameworkCore": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "GrafanaLoki",
        "Args": {
          "uri": "http://localhost:3100",
          "labels": [
            {
              "key": "app",
              "value": "cleanhr-api"
            },
            {
              "key": "environment",
              "value": "Development"
            }
          ]
        }
      }
    ],
    "Enrich": [
      "FromLogContext",
      "WithEnvironmentName",
      "WithMachineName",
      "WithThreadId"
    ],
    "Properties": {
      "Application": "CleanHrApi"
    }
  }
}
```

## 🐳 Docker Infrastructure

### Loki Container

Added to `docker-compose.yml`:

```yaml
loki:
  image: grafana/loki:latest
  container_name: loki
  command: [ "-config.file=/etc/loki/local-config.yaml" ]
  volumes:
    - ./Observability/loki-config.yaml:/etc/loki/local-config.yaml
    - loki-data:/loki
  ports:
    - "3100:3100"   # Loki HTTP API
  networks:
    - observability
```

### Grafana Datasource

Updated `grafana-datasources.yaml` to include Loki:

```yaml
- name: Loki
  type: loki
  access: proxy
  url: http://loki:3100
  uid: loki
  editable: true
  jsonData:
    maxLines: 1000
    derivedFields:
      - datasourceUid: tempo
        matcherRegex: "traceID=(\\w+)"
        name: TraceID
        url: "$${__value.raw}"
```

## 🚀 Usage

### Starting the Stack

```bash
cd src/ServerApp/Presentation/CleanHr.Api
docker compose up -d
```

This starts:
- SQL Server (port 1440)
- Grafana Tempo (port 3200, 4317, 4318)
- **Grafana Loki** (port 3100)
- Grafana (port 3000)

### Running the Application

```bash
dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj
```

### Viewing Logs in Grafana

1. Open Grafana: http://localhost:3000
2. Go to **Explore** → Select **Loki** datasource
3. Use LogQL queries:
   - All logs: `{app="cleanhr-api"}`
   - By environment: `{app="cleanhr-api", environment="Development"}`
   - By level: `{app="cleanhr-api"} |= "Error"`
   - Search text: `{app="cleanhr-api"} |~ "(?i)exception"`

## 📊 Log Levels

Serilog supports standard log levels:

```csharp
Log.Verbose("Detailed trace information");
Log.Debug("Debugging information");
Log.Information("General informational messages");
Log.Warning("Warning messages for potentially harmful situations");
Log.Error(exception, "Error messages with exception details");
Log.Fatal(exception, "Critical errors causing application shutdown");
```

## 🔍 Structured Logging Examples

### Basic Logging

```csharp
Log.Information("User {UserId} logged in at {LoginTime}", userId, DateTime.UtcNow);
```

### Logging with Context

```csharp
using (LogContext.PushProperty("OrderId", orderId))
{
    Log.Information("Processing order");
    // All logs within this scope will have OrderId property
}
```

### Exception Logging

```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    Log.Error(ex, "Failed to process order {OrderId}", orderId);
    throw;
}
```

## 🔗 Integration with Traces

Logs are automatically correlated with traces in Grafana:
- Each log entry can link to its corresponding trace
- Traces can show related log entries
- Use the same correlation ID for both logs and traces

## 📈 Best Practices

1. **Use structured logging**: Always pass parameters separately, not in the message string
   - ✅ `Log.Information("User {UserId} created", userId)`
   - ❌ `Log.Information($"User {userId} created")`

2. **Use appropriate log levels**:
   - Verbose/Debug: Development details
   - Information: Normal application flow
   - Warning: Unexpected but handled situations
   - Error: Failures that need attention
   - Fatal: Application-level failures

3. **Add context with enrichers**:
   ```csharp
   using (LogContext.PushProperty("RequestId", requestId))
   {
       // Your code
   }
   ```

4. **Log exceptions properly**:
   ```csharp
   Log.Error(ex, "Operation failed for {EntityId}", entityId);
   ```

## 🛠️ Troubleshooting

### Logs Not Appearing in Loki

1. **Check Loki is running**:
   ```bash
   docker ps | grep loki
   curl http://localhost:3100/ready
   ```

2. **Verify network connectivity**:
   - From the application to Loki on port 3100
   - Check Docker network: `docker network inspect cleanhrapi_observability`

3. **Check application logs**:
   - Look for Serilog errors in console output
   - Verify Loki sink is configured correctly

### Viewing Loki Logs

```bash
docker logs loki
```

### Testing Loki Endpoint

```bash
curl -G -s "http://localhost:3100/loki/api/v1/query" \
  --data-urlencode 'query={app="cleanhr-api"}' \
  | jq
```

## 📚 Resources

- [Serilog Documentation](https://serilog.net/)
- [Grafana Loki Documentation](https://grafana.com/docs/loki/latest/)
- [LogQL Query Language](https://grafana.com/docs/loki/latest/logql/)
- [Serilog Best Practices](https://github.com/serilog/serilog/wiki/Getting-Started)
