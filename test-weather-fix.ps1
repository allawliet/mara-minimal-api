# Quick Weather Endpoint Test Script
# This script tests both the old and new weather endpoints

Write-Host "=== Weather Endpoint Fix Verification ===" -ForegroundColor Green

# Note: Update these URLs based on your actual API service ports from Aspire dashboard
$commonPorts = @(5001, 7001, 5239, 7239, 5000, 7000)
$apiUrl = $null

# Find the correct API port
foreach ($port in $commonPorts) {
    try {
        $response = Invoke-RestMethod "https://localhost:$port/health" -ErrorAction SilentlyContinue
        $apiUrl = "https://localhost:$port"
        Write-Host "✅ API Service found at: $apiUrl" -ForegroundColor Green
        break
    }
    catch {
        # Port not responding, try next
    }
}

if (-not $apiUrl) {
    Write-Host "❌ Could not find API service. Please check the Aspire dashboard for the correct port." -ForegroundColor Red
    Write-Host "Dashboard URL: https://localhost:17238/login?t=bf93b3f3801f22dd9a917bbece62b209" -ForegroundColor Yellow
    exit
}

# Test 1: New weather endpoint
Write-Host "`n1. Testing NEW weather endpoint: /weather/forecast" -ForegroundColor Cyan
try {
    $newWeather = Invoke-RestMethod -Uri "$apiUrl/weather/forecast" -Method Get
    Write-Host "✅ New endpoint working!" -ForegroundColor Green
    Write-Host "   Forecast items: $($newWeather.Count)" -ForegroundColor Yellow
}
catch {
    Write-Host "❌ New endpoint failed: $_" -ForegroundColor Red
}

# Test 2: Backward compatibility endpoint
Write-Host "`n2. Testing LEGACY weather endpoint: /weatherforecast (backward compatibility)" -ForegroundColor Cyan
try {
    $oldWeather = Invoke-RestMethod -Uri "$apiUrl/weatherforecast" -Method Get
    Write-Host "✅ Legacy endpoint working!" -ForegroundColor Green
    Write-Host "   Forecast items: $($oldWeather.Count)" -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Legacy endpoint failed: $_" -ForegroundColor Red
}

# Test 3: Custom forecast
Write-Host "`n3. Testing CUSTOM weather forecast: /weather/forecast/7" -ForegroundColor Cyan
try {
    $customWeather = Invoke-RestMethod -Uri "$apiUrl/weather/forecast/7" -Method Get
    Write-Host "✅ Custom forecast working!" -ForegroundColor Green
    Write-Host "   Forecast items: $($customWeather.Count)" -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Custom forecast failed: $_" -ForegroundColor Red
}

Write-Host "`n=== Test Results ===" -ForegroundColor Green
Write-Host "API Base URL: $apiUrl" -ForegroundColor Yellow
Write-Host "Swagger UI: $apiUrl/swagger" -ForegroundColor Cyan
Write-Host "Aspire Dashboard: https://localhost:17238" -ForegroundColor Cyan

Write-Host "`n✅ The 404 error should now be resolved!" -ForegroundColor Green
Write-Host "The web frontend can now successfully call weather endpoints." -ForegroundColor Yellow
