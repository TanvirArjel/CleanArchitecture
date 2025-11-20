# 🔍 Troubleshooting: No Traces Visible in Grafana

## Issue
Traces are not appearing in Grafana after starting the application.

## Root Cause Analysis

Based on the Tempo logs, searches are being made but returning `inspected_traces=0 inspected_spans=0`, which means **no traces are being received by Tempo from the application**.

## Common Causes & Solutions

### 1. ⚠️ Application Not Running
**Most Common Issue**: The application must be actively running to send traces.

**Solution:**
```bash
cd /Users/tanvirarjel/Works/Development/GitHub/CleanArchitecture
dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj
```

Wait for: `Now listening on: http://localhost:5100`

### 2. ⚠️ No Requests Being Made
Traces are only created when HTTP requests are processed.

**Solution:** Make some requests to the API:
```bash
# Test swagger page
curl http://localhost:5100/swagger/index.html

# Test API endpoints
curl http://localhost:5100/api/v1/your-endpoint
```

### 3. ⚠️ Service Name Mismatch
**Check:** The service name in config is `CleanHrApi` (without dot or space).

**In Grafana, search for:**
- Service name: `CleanHrApi` (exact match, case-sensitive)
- NOT: `CleanHr.Api` or `Clean HR Api`

**TraceQL Query:**
```
{ .service.name = "CleanHrApi" }
```

### 4. ⚠️ Time Range in Grafana
**Problem:** Grafana might be searching an old time range.

**Solution:** In Grafana Explore:
1. Click the time range picker (top right)
2. Select "Last 5 minutes" or "Last 15 minutes"
3. Click "Run query" again

### 5. ⚠️ OTLP Endpoint Not Reachable
**Check if Tempo's OTLP endpoint is accessible:**
```bash
# Should show Tempo is listening
docker logs tempo 2>&1 | grep "Starting GRPC server"
```

Expected output:
```
level=info msg="Starting GRPC server" component=tempo endpoint=[::]:4317
level=info msg="Starting HTTP server" component=tempo endpoint=[::]:4318
```

### 6. ⚠️ Verify Console Exporter Output
The application has a Console Exporter that prints traces to the console.

**When the app is running and receiving requests, you should see:**
```
Activity.TraceId:            abc123def456...
Activity.SpanId:             789xyz...
Activity.DisplayName:        GET /swagger/index.html
Activity.Kind:               Server
Activity.Tags:
    http.method: GET
    http.scheme: http
    http.status_code: 200
```

**If you DON'T see this:** OpenTelemetry is not capturing traces. Check for startup errors.

**If you DO see this:** Traces are being created! They should be in Tempo/Grafana.

---

## Step-by-Step Verification Process

### Step 1: Start Infrastructure
```bash
docker compose -f docker-compose.observability.yml up -d
docker ps  # Verify tempo and grafana are running
```

### Step 2: Start Application
```bash
dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj
```

Watch for:
- ✅ "Now listening on: http://localhost:5100"
- ✅ No OpenTelemetry-related errors

### Step 3: Generate Traces
In a new terminal:
```bash
# Make multiple requests
for i in {1..5}; do
  curl -s http://localhost:5100/swagger/index.html > /dev/null
  echo "Request $i sent"
  sleep 1
done
```

### Step 4: Check Console Output
In the application console, you should see trace output like:
```
Activity.TraceId:            ...
Activity.SpanId:             ...
Activity.DisplayName:        GET /swagger/index.html
```

### Step 5: Wait for Indexing
Wait 5-10 seconds for Tempo to index the traces.

### Step 6: Search in Grafana
1. Open: http://localhost:3000
2. Click: **Explore** (compass icon)
3. Datasource: **Tempo**
4. Click: **Search** tab
5. Service Name: Type `CleanHrApi`
6. Time range: **Last 15 minutes**
7. Click: **Run query**

### Step 7: Alternative - Use TraceQL
In the **TraceQL** tab, enter:
```
{ .service.name = "CleanHrApi" }
```
Click **Run query**

---

## Debugging Checklist

- [ ] Docker containers (tempo, grafana) are running
- [ ] Application is running (dotnet run)
- [ ] Made HTTP requests to the application
- [ ] Console shows trace output (Activity.TraceId, etc.)
- [ ] Waited at least 10 seconds after making requests
- [ ] Grafana time range set to "Last 15 minutes"
- [ ] Searching for exact service name: `CleanHrApi`
- [ ] Tempo logs show no errors: `docker logs tempo --tail 20`

---

## Quick Test Script

Run this automated verification:
```bash
chmod +x verify-tracing.sh
./verify-tracing.sh
```

---

## If Traces Still Don't Appear

### Check Application Logs for Errors
Look for any errors related to:
- `OpenTelemetry`
- `OTLP`
- `Exporter`
- Connection refused
- DNS errors

### Verify Network Configuration
If running in Docker, ensure the application can reach `localhost:4317`.

**For Dockerized apps**, change the endpoint in `appsettings.json`:
```json
"OpenTelemetry": {
  "OtlpEndpoint": "http://host.docker.internal:4317"  // For Docker Desktop
}
```

### Enable More Logging
Add to `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "OpenTelemetry": "Debug",
      "Microsoft": "Warning"
    }
  }
}
```

### Restart Everything
```bash
# Stop all
docker compose -f docker-compose.observability.yml down
# Kill application (Ctrl+C)

# Start fresh
docker compose -f docker-compose.observability.yml up -d
sleep 5
dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj
```

---

## Expected Behavior

✅ **When Working Correctly:**
1. Application starts without OpenTelemetry errors
2. Console shows Activity trace output for each HTTP request
3. Traces appear in Grafana within 5-10 seconds
4. Can filter by service name, HTTP method, status code, etc.
5. Can click on traces to see detailed span information

---

## Contact Points

If still having issues, check:
1. Application console output - any errors?
2. `docker logs tempo` - any errors receiving traces?
3. `docker logs grafana` - any datasource errors?

Remember: **The application must be running and processing requests** for traces to be generated!
