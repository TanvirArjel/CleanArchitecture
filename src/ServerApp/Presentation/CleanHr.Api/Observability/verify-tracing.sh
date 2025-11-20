#!/bin/bash

echo "🔍 OpenTelemetry Tracing - Quick Verification Guide"
echo "=================================================="
echo ""

# Check if Tempo is running
echo "Step 1: Checking Tempo status..."
if docker ps | grep -q tempo; then
    echo "✅ Tempo is running"
else
    echo "❌ Tempo is NOT running"
    echo "   Start it with: docker compose -f docker-compose.observability.yml up -d"
    exit 1
fi

echo ""
echo "Step 2: Starting the CleanHr.Api application..."
echo "   Run this command in a separate terminal:"
echo "   cd /Users/tanvirarjel/Works/Development/GitHub/CleanArchitecture"
echo "   dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj"
echo ""
echo "   Wait for the message: 'Now listening on: http://localhost:5100'"
echo ""

read -p "Press Enter once the application is running..."

echo ""
echo "Step 3: Making test requests to generate traces..."
echo ""

# Test if app is running
if curl -s http://localhost:5100/healthz > /dev/null 2>&1; then
    echo "✅ Application is responding"
    
    echo ""
    echo "Making test requests..."
    
    # Make a few requests
    for i in {1..3}; do
        echo "   Request $i..."
        response=$(curl -s -w "\nHTTP_CODE:%{http_code}" http://localhost:5100/swagger/index.html 2>&1)
        http_code=$(echo "$response" | grep "HTTP_CODE" | cut -d: -f2)
        
        if [ "$http_code" = "200" ]; then
            echo "   ✅ Request $i successful (HTTP $http_code)"
        else
            echo "   ⚠️  Request $i returned HTTP $http_code"
        fi
        sleep 1
    done
    
    echo ""
    echo "Step 4: Waiting for traces to be ingested (5 seconds)..."
    sleep 5
    
    echo ""
    echo "Step 5: Checking for traces in Tempo..."
    
    # Search for traces
    search_result=$(curl -s "http://localhost:3200/api/search?q={.service.name=%22CleanHrApi%22}&limit=10&start=$(($(date +%s) - 600))&end=$(date +%s)" 2>&1)
    
    if echo "$search_result" | grep -q "traces"; then
        echo "✅ Traces found in Tempo!"
        echo ""
        echo "View them in Grafana:"
        echo "1. Open: http://localhost:3000"
        echo "2. Go to: Explore → Select Tempo"
        echo "3. Search for service: CleanHrApi"
        echo "4. Or use TraceQL: { .service.name = \"CleanHrApi\" }"
    else
        echo "⚠️  No traces found yet in Tempo"
        echo ""
        echo "Troubleshooting:"
        echo "1. Check application console for OpenTelemetry logs"
        echo "2. Look for trace output (Console Exporter should show traces)"
        echo "3. Check application logs for any OTLP export errors"
        echo ""
        echo "If you see traces in the console but not in Grafana:"
        echo "- Wait a few more seconds for indexing"
        echo "- Check the service name matches: 'CleanHrApi'"
        echo "- Verify the time range in Grafana (try 'Last 15 minutes')"
    fi
    
else
    echo "❌ Application is NOT responding on http://localhost:5100"
    echo ""
    echo "Please start the application first:"
    echo "   dotnet run --project src/ServerApp/Presentation/CleanHr.Api/CleanHr.Api.csproj"
    exit 1
fi

echo ""
echo "=================================================="
echo "✨ Verification Complete!"
echo ""
echo "📝 What to look for in the application console:"
echo "   - Lines starting with 'Activity.TraceId:' (from Console Exporter)"
echo "   - Lines starting with 'Activity.DisplayName:' showing HTTP requests"
echo "   - Tags like 'http.method', 'http.status_code', etc."
echo ""
echo "If you see these in the console, OpenTelemetry is working!"
echo "Traces should appear in Grafana within a few seconds."
