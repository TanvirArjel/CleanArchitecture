# OpenTelemetry Tracing with Grafana Tempo

This project includes distributed tracing using OpenTelemetry (OTEL) and Grafana Tempo for observability.

## 🎯 What is Distributed Tracing?

Distributed tracing helps you monitor and troubleshoot microservices-based applications by tracking requests as they flow through various services and components. It provides insights into:
- Request latency and performance bottlenecks
- Service dependencies
- Error rates and exception details
- Database query performance
- HTTP request/response details

## 🏗️ Architecture

The observability stack consists of:
- **OpenTelemetry SDK**: Instruments the application to collect traces
- **Grafana Tempo**: Stores and queries distributed traces
- **Grafana**: Visualizes traces and creates dashboards

## 🚀 Getting Started

### 1. Start the Observability Stack

Run the following command from the project root:

```bash
docker-compose -f docker-compose.observability.yml up -d
```

This will start:
- **Grafana Tempo** on `http://localhost:3200` (OTLP gRPC on port 4317, HTTP on port 4318)
- **Grafana** on `http://localhost:3000`

### 2. Run Your Application

Start the CleanHr.Api application as usual. It will automatically send traces to Tempo.

### 3. View Traces in Grafana

1. Open your browser and navigate to `http://localhost:3000`
2. Click on "Explore" in the left sidebar
3. Select "Tempo" as the data source
4. Use the "Search" tab to query traces by:
   - Service Name: `CleanHr.Api`
   - Operation Name
   - Tags
   - Duration
   - Time range

## 📊 What Gets Traced?

The application automatically instruments:

### ASP.NET Core Requests
- HTTP method, path, and status code
- Request duration
- Exception details (if any occur)
- Excludes health check endpoints (`/healthz`)

### HTTP Client Calls
- Outgoing HTTP requests
- Request/response details
- Exception tracking

### SQL Database Queries
- SQL commands and queries
- Query execution time
- Connection details
- Exception tracking

## ⚙️ Configuration

### appsettings.json

```json
{
  "OpenTelemetry": {
    "ServiceName": "CleanHr.Api",
    "OtlpEndpoint": "http://localhost:4317"
  }
}
```

- **ServiceName**: Identifies your service in traces
- **OtlpEndpoint**: The endpoint where traces are sent (Tempo's OTLP gRPC receiver)

### Environment-Specific Configuration

For production, update the `OtlpEndpoint` in `appsettings.Production.json`:

```json
{
  "OpenTelemetry": {
    "OtlpEndpoint": "http://tempo-service:4317"
  }
}
```

## 🔍 Understanding Traces

### Trace Components

- **Trace**: Represents the entire journey of a request through the system
- **Span**: A single unit of work within a trace (e.g., HTTP request, database query)
- **Attributes**: Key-value pairs that provide context (e.g., HTTP method, SQL query)
- **Events**: Timestamped records within a span (e.g., exceptions)

### Example Trace Flow

```
HTTP Request → ASP.NET Core Middleware → Controller → Service Layer → Database Query
     |                |                      |            |              |
   Span 1          Span 2               Span 3       Span 4         Span 5
```

## 📈 Best Practices

### 1. Add Custom Spans for Business Logic

You can add custom instrumentation to trace specific business operations:

```csharp
using System.Diagnostics;

public class DepartmentService
{
    private static readonly ActivitySource ActivitySource = new("CleanHr.Application");

    public async Task<Department> CreateDepartment(CreateDepartmentRequest request)
    {
        using var activity = ActivitySource.StartActivity("CreateDepartment");
        activity?.SetTag("department.name", request.Name);
        
        try
        {
            // Your business logic here
            var department = await _repository.AddAsync(entity);
            
            activity?.SetTag("department.id", department.Id);
            return department;
        }
        catch (Exception ex)
        {
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.RecordException(ex);
            throw;
        }
    }
}
```

Don't forget to register the ActivitySource in Program.cs:

```csharp
.WithTracing(tracing => tracing
    .AddSource("CleanHr.Application") // Add this line
    .AddAspNetCoreInstrumentation()
    // ... other configurations
)
```

### 2. Use Tags for Searchability

Add meaningful tags to help filter and search traces:

```csharp
activity?.SetTag("user.id", userId);
activity?.SetTag("tenant.id", tenantId);
activity?.SetTag("operation.type", "create");
```

### 3. Record Exceptions

Always record exceptions in custom spans:

```csharp
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
    activity?.RecordException(ex);
    throw;
}
```

## 🛠️ Troubleshooting

### Traces Not Appearing

1. **Check if Tempo is running:**
   ```bash
   docker ps | grep tempo
   ```

2. **Check Tempo logs:**
   ```bash
   docker logs tempo
   ```

3. **Verify the endpoint in appsettings.json** matches the Tempo OTLP receiver

4. **Check application logs** for OpenTelemetry errors

### Console Exporter

The application also exports traces to the console for debugging. Check your application logs to see trace output.

To disable console exporter in production, modify Program.cs:

```csharp
.WithTracing(tracing => tracing
    // ... other configurations
    .AddOtlpExporter()
    // Remove .AddConsoleExporter() for production
)
```

## 🌐 Advanced Configuration

### Sampling

To reduce trace volume in high-traffic scenarios, configure sampling:

```csharp
.WithTracing(tracing => tracing
    .SetSampler(new TraceIdRatioBasedSampler(0.1)) // Sample 10% of traces
    // ... other configurations
)
```

### Resource Attributes

Add additional resource attributes for better context:

```csharp
.ConfigureResource(resource => resource
    .AddService(serviceName: "CleanHr.Api")
    .AddAttributes(new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName,
        ["host.name"] = Environment.MachineName,
        ["service.version"] = "1.0.0"
    }))
```

## 📚 Additional Resources

- [OpenTelemetry Documentation](https://opentelemetry.io/docs/)
- [Grafana Tempo Documentation](https://grafana.com/docs/tempo/latest/)
- [OpenTelemetry .NET SDK](https://github.com/open-telemetry/opentelemetry-dotnet)
- [Grafana Explore Guide](https://grafana.com/docs/grafana/latest/explore/)

## 🔐 Security Considerations

1. **Production Deployment**: Use authentication for Grafana in production
2. **Network Security**: Ensure Tempo endpoint is only accessible within your network
3. **Data Retention**: Configure appropriate retention policies to manage storage costs
4. **Sensitive Data**: Avoid logging sensitive information (passwords, tokens) in traces

## 🧹 Cleanup

To stop and remove the observability stack:

```bash
docker-compose -f docker-compose.observability.yml down -v
```

The `-v` flag removes the volumes, deleting all stored traces.
