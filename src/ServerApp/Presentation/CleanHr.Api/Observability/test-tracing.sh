#!/bin/bash

echo "🚀 Testing OpenTelemetry Tracing Setup"
echo "======================================"
echo ""

echo "1. Checking if Tempo is running..."
if docker ps | grep -q tempo; then
    echo "✅ Tempo is running"
    echo "   - OTLP gRPC endpoint: http://localhost:4317"
    echo "   - OTLP HTTP endpoint: http://localhost:4318"
    echo "   - Query endpoint: http://localhost:3200"
else
    echo "❌ Tempo is not running"
    echo "   Start it with: docker compose -f docker-compose.observability.yml up -d"
    exit 1
fi

echo ""
echo "2. Checking if Grafana is running..."
if docker ps | grep -q grafana; then
    echo "✅ Grafana is running"
    echo "   - UI: http://localhost:3000"
else
    echo "❌ Grafana is not running"
fi

echo ""
echo "3. Checking Tempo health..."
tempo_health=$(curl -s http://localhost:3200/ready 2>&1)
if echo "$tempo_health" | grep -q "ready"; then
    echo "✅ Tempo is healthy and ready to receive traces"
else
    echo "⚠️  Tempo status: $tempo_health"
fi

echo ""
echo "4. Sending a test trace using OpenTelemetry Protocol..."
echo ""

# Create a simple test trace using curl to send OTLP data
# This simulates what the application will do
cat <<'EOF' > /tmp/test-trace.json
{
  "resourceSpans": [{
    "resource": {
      "attributes": [{
        "key": "service.name",
        "value": { "stringValue": "test-service" }
      }]
    },
    "scopeSpans": [{
      "scope": {
        "name": "manual-test"
      },
      "spans": [{
        "traceId": "5B8EFFF798038103D269B633813FC60C",
        "spanId": "EEE19B7EC3C1B174",
        "name": "test-span",
        "kind": 1,
        "startTimeUnixNano": "1605544146000000000",
        "endTimeUnixNano": "1605544146000100000",
        "attributes": [{
          "key": "http.method",
          "value": { "stringValue": "GET" }
        }]
      }]
    }]
  }]
}
EOF

echo "   Sending test trace to Tempo..."
response=$(curl -X POST http://localhost:4318/v1/traces \
    -H "Content-Type: application/json" \
    -d @/tmp/test-trace.json \
    -w "\nHTTP_CODE:%{http_code}" \
    -s 2>&1)

http_code=$(echo "$response" | grep "HTTP_CODE" | cut -d: -f2)

if [ "$http_code" = "200" ] || [ "$http_code" = "202" ]; then
    echo "✅ Test trace sent successfully (HTTP $http_code)"
    echo ""
    echo "5. Verifying trace ingestion..."
    sleep 2
    
    # Query Tempo for the trace
    trace_result=$(curl -s "http://localhost:3200/api/traces/5B8EFFF798038103D269B633813FC60C" 2>&1)
    
    if echo "$trace_result" | grep -q "test-span"; then
        echo "✅ Trace successfully ingested and queryable!"
    else
        echo "⚠️  Trace sent but not yet queryable (may take a few seconds)"
    fi
else
    echo "❌ Failed to send test trace (HTTP $http_code)"
    echo "Response: $response"
fi

echo ""
echo "======================================"
echo "📊 Next Steps:"
echo "1. Start your CleanHr.Api application"
echo "2. Make some API requests"
echo "3. Open Grafana at http://localhost:3000"
echo "4. Go to Explore → Select Tempo"
echo "5. Search for traces from 'CleanHr.Api' service"
echo ""
echo "For detailed documentation, see OBSERVABILITY.md"
