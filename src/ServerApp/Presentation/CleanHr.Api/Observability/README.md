# 📊 Observability - OpenTelemetry, Grafana Tempo & Serilog with Loki

This folder contains all the configuration and documentation for distributed tracing with OpenTelemetry and structured logging with Serilog, using Grafana for visualization.

## 📁 Contents

### Configuration Files
- **`docker-compose.yml`** (in CleanHr.Api directory) - Docker Compose file to start infrastructure
- **`tempo-config.yaml`** - Grafana Tempo configuration for distributed tracing
- **`loki-config.yaml`** - Grafana Loki configuration for log aggregation
- **`grafana-datasources.yaml`** - Grafana datasource configuration (auto-provisions Tempo and Loki)

### Documentation
- **`OBSERVABILITY.md`** - Complete guide on OpenTelemetry tracing implementation
- **`TRACING_VERIFICATION.md`** - Verification results and testing guide
- **`TROUBLESHOOTING_TRACING.md`** - Common issues and solutions

### Scripts
- **`test-tracing.sh`** - Simple test to verify Tempo is receiving traces
- **`verify-tracing.sh`** - Interactive verification script

## 🚀 Quick Start

### 1. Start the Observability Stack (with SQL Server)

From the CleanHr.Api directory:
```bash
cd src/ServerApp/Presentation/CleanHr.Api
docker compose up -d
```

Or from the project root:
```bash
docker compose -f src/ServerApp/Presentation/CleanHr.Api/docker-compose.yml up -d
```

This starts:
- **SQL Server** on port 1440
- **Grafana Tempo** on port 3200 (OTLP on 4317/4318)
- **Grafana Loki** on port 3100 (for logs)
- **Grafana** on port 3000

### 2. Run Your Application

```bash
cd /path/to/project/root
dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj
```

The application will connect to SQL Server at `localhost,1440` using the `DockerDbConnection` connection string.

### 3. Generate Traces and Logs

Make some HTTP requests to your API:
```bash
curl http://localhost:5100/swagger/index.html
```

The application will:
- Send distributed traces to Tempo
- Send structured logs to Loki
- Display logs in the console

### 4. View Traces and Logs

Open Grafana at: http://localhost:3000

**View Traces:**
- Go to: **Explore** → **Tempo**
- Search for service: **CleanHrApi**

**View Logs:**
- Go to: **Explore** → **Loki**
- Query logs with: `{app="cleanhr-api"}`
- Filter by environment: `{app="cleanhr-api", environment="Development"}`

## 🗄️ SQL Server Connection

The docker-compose.yml includes SQL Server configured to match your `DockerDbConnection`:

```
Server: localhost,1440
Database: CleanHrDb
User: sa
Password: Pass@1234
```

You can connect with any SQL client using these credentials.

## 📚 Documentation

For detailed information, see:
- [OBSERVABILITY.md](./OBSERVABILITY.md) - Full implementation guide
- [TROUBLESHOOTING_TRACING.md](./TROUBLESHOOTING_TRACING.md) - If traces aren't appearing

## 🔧 What's Configured

**Distributed Tracing (OpenTelemetry → Tempo):**
- ✅ All HTTP requests and responses
- ✅ SQL Database queries
- ✅ Outgoing HTTP client calls
- ✅ Exceptions and errors

**Structured Logging (Serilog → Loki):**
- ✅ Structured log messages with context
- ✅ Automatic enrichment with environment, machine name, and thread ID
- ✅ Console output for development
- ✅ Loki sink for centralized log aggregation
- ✅ Correlation between logs and traces in Grafana

## 🛑 Stop Everything

From the CleanHr.Api directory:
```bash
cd src/ServerApp/Presentation/CleanHr.Api
docker compose down
```

Or from the project root:
```bash
docker compose -f src/ServerApp/Presentation/CleanHr.Api/docker-compose.yml down
```

To remove volumes as well:
```bash
docker compose -f src/ServerApp/Presentation/CleanHr.Api/docker-compose.yml down -v
```
