# 🚀 Quick Start - Structured Logging with Serilog & Loki

## ✅ What's Been Added

### Packages Installed
- **Serilog.AspNetCore** (9.0.0)
- **Serilog.Sinks.Grafana.Loki** (8.3.1)
- **Serilog.Enrichers.Environment** (3.0.1)
- **Serilog.Enrichers.Thread** (4.0.0)

### Infrastructure
- **Grafana Loki** container added to docker-compose.yml
- Loki datasource auto-configured in Grafana
- Log correlation with distributed traces

## 🎯 Start the Full Stack

### 1. Start Docker Services

**Note:** First, ensure Docker Desktop is running, then:

```bash
cd src/ServerApp/Presentation/CleanHr.Api
docker compose up -d
```

This starts:
- ✅ **SQL Server** on port 1440
- ✅ **Grafana Tempo** on port 3200, 4317, 4318 (traces)
- ✅ **Grafana Loki** on port 3100 (logs)
- ✅ **Grafana** on port 3000 (visualization)

### 2. Run the Application

```bash
# From project root
dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj
```

You should see logs in the console like:
```
[09:15:23 INF] Starting CleanHR API application
[09:15:24 INF] Application started. Press Ctrl+C to shut down.
```

### 3. Generate Some Logs

```bash
# Make requests to your API
curl http://localhost:5100/swagger/index.html
curl http://localhost:5100/api/employees
```

### 4. View Logs in Grafana

1. Open **Grafana**: http://localhost:3000
2. Click **Explore** (compass icon in left sidebar)
3. Select **Loki** from the datasource dropdown
4. Try these queries:

**All application logs:**
```logql
{app="cleanhr-api"}
```

**Logs from Development environment:**
```logql
{app="cleanhr-api", environment="Development"}
```

**Error logs only:**
```logql
{app="cleanhr-api"} |= "Error"
```

**Warning and above:**
```logql
{app="cleanhr-api"} |~ "(?i)(warning|error|fatal)"
```

**Search for specific text:**
```logql
{app="cleanhr-api"} |= "Employee"
```

## 📊 View Traces

1. In Grafana, select **Tempo** from datasource dropdown
2. Search for service: **CleanHrApi**
3. Click on any trace to see details
4. Notice the "Logs for this span" button - this correlates traces with logs!

## 🔍 Log Examples in Code

The application already uses Serilog everywhere. Here are some examples:

### Information Logs
```csharp
// Logs are automatically structured
_logger.LogInformation("User {UserId} accessed resource {ResourceId}", userId, resourceId);
```

### Error Logs with Exception
```csharp
try
{
    // Your code
}
catch (Exception ex)
{
    _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
    throw;
}
```

### Debug Logs
```csharp
_logger.LogDebug("Processing request with parameters: {@Parameters}", parameters);
```

## 🛠️ Troubleshooting

### Docker Not Starting

If you see "Cannot connect to the Docker daemon":
1. Open Docker Desktop
2. Wait for it to fully start (green icon in menu bar)
3. Try again: `docker compose up -d`

### Port Already in Use

If port 3100 is in use:
```bash
# Kill the process using port 3100
lsof -ti:3100 | xargs kill -9

# Restart services
docker compose up -d
```

### Logs Not Appearing in Loki

1. **Check Loki is running:**
   ```bash
   docker ps | grep loki
   ```

2. **Check Loki health:**
   ```bash
   curl http://localhost:3100/ready
   ```

3. **View Loki logs:**
   ```bash
   docker logs loki
   ```

4. **Test pushing a log manually:**
   ```bash
   curl -H "Content-Type: application/json" \
     -XPOST "http://localhost:3100/loki/api/v1/push" \
     --data '{"streams": [{"stream": {"app": "test"}, "values": [["'"$(date +%s)000000000"'", "test message"]]}]}'
   ```

### Check All Services Status

```bash
cd src/ServerApp/Presentation/CleanHr.Api
docker compose ps
```

You should see all 4 services running (sqlserver, tempo, loki, grafana).

## 📚 More Information

See detailed documentation:
- **SERILOG_LOKI_SETUP.md** - Complete setup and configuration guide
- **OBSERVABILITY.md** - OpenTelemetry tracing guide
- **README.md** - General observability overview

## 🛑 Stop Everything

```bash
cd src/ServerApp/Presentation/CleanHr.Api
docker compose down

# To also remove data volumes:
docker compose down -v
```

## 🎉 Success Indicators

You'll know everything is working when:
- ✅ Application starts without errors
- ✅ Console shows structured log messages
- ✅ Loki query in Grafana returns log entries
- ✅ Tempo query shows traces
- ✅ You can correlate logs with traces in Grafana
