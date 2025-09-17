# Weather Module Test Script
# This script tests the new CRUD functionality in the Weather module

# Get the API URL from Aspire (you may need to update this based on the dashboard)
$apiUrl = "https://localhost:7535"  # Update this with the actual API URL from Aspire dashboard

Write-Host "=== Weather Module CRUD Testing ===" -ForegroundColor Green
Write-Host "API URL: $apiUrl" -ForegroundColor Yellow

# Skip certificate validation for testing
if (-not ([System.Management.Automation.PSTypeName]'ServerCertificateValidationCallback').Type) {
    $certCallback = @"
        using System;
        using System.Net;
        using System.Net.Security;
        using System.Security.Cryptography.X509Certificates;
        public class ServerCertificateValidationCallback
        {
            public static void Ignore()
            {
                if(ServicePointManager.ServerCertificateValidationCallback ==null)
                {
                    ServicePointManager.ServerCertificateValidationCallback += 
                        delegate
                        (
                            Object obj, 
                            X509Certificate certificate, 
                            X509Chain chain, 
                            SslPolicyErrors errors
                        )
                        {
                            return true;
                        };
                }
            }
        }
"@
    Add-Type $certCallback
}
[ServerCertificateValidationCallback]::Ignore()

# Test 1: Get all weather forecasts (should show sample data)
Write-Host "`n1. Testing GET /weather/ (Get all forecasts)..." -ForegroundColor Cyan
try {
    $forecasts = Invoke-RestMethod -Uri "$apiUrl/weather/" -Method Get
    Write-Host "✅ Retrieved $($forecasts.Count) weather forecasts" -ForegroundColor Green
    Write-Host "Sample forecast:" -ForegroundColor Yellow
    $forecasts[0] | ConvertTo-Json
}
catch {
    Write-Host "❌ Failed to get weather forecasts: $_" -ForegroundColor Red
    Write-Host "Error details: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Get weather forecast by location
Write-Host "`n2. Testing GET /weather/location/London..." -ForegroundColor Cyan
try {
    $londonForecasts = Invoke-RestMethod -Uri "$apiUrl/weather/location/London" -Method Get
    Write-Host "✅ Retrieved $($londonForecasts.Count) London forecasts" -ForegroundColor Green
    if ($londonForecasts.Count -gt 0) {
        $londonForecasts[0] | ConvertTo-Json
    }
}
catch {
    Write-Host "❌ Failed to get London forecasts: $_" -ForegroundColor Red
}

# Test 3: Get generated forecast (legacy endpoint)
Write-Host "`n3. Testing GET /weather/forecast (Legacy endpoint)..." -ForegroundColor Cyan
try {
    $generatedForecast = Invoke-RestMethod -Uri "$apiUrl/weather/forecast" -Method Get
    Write-Host "✅ Generated forecast retrieved successfully" -ForegroundColor Green
    Write-Host "Generated $($generatedForecast.Count) forecast entries" -ForegroundColor Yellow
    $generatedForecast[0] | ConvertTo-Json
}
catch {
    Write-Host "❌ Failed to get generated forecast: $_" -ForegroundColor Red
}

# Test 4: Test backward compatibility endpoint
Write-Host "`n4. Testing GET /weatherforecast (Backward compatibility)..." -ForegroundColor Cyan
try {
    $compatForecast = Invoke-RestMethod -Uri "$apiUrl/weatherforecast" -Method Get
    Write-Host "✅ Backward compatibility endpoint works!" -ForegroundColor Green
    Write-Host "Retrieved $($compatForecast.Count) forecast entries" -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Backward compatibility endpoint failed: $_" -ForegroundColor Red
}

# Test 5: Create a new weather forecast (requires authentication)
Write-Host "`n5. Testing POST /weather/ (Create forecast - requires auth)..." -ForegroundColor Cyan

# First, let's try to register and login to get a token
$registerData = @{
    username = "weatheruser"
    email = "weather@test.com"
    password = "Weather123!"
} | ConvertTo-Json

$headers = @{
    "Content-Type" = "application/json"
}

try {
    # Try to register (might fail if user exists, that's OK)
    try {
        $registerResponse = Invoke-RestMethod -Uri "$apiUrl/auth/register" -Method Post -Body $registerData -Headers $headers
        Write-Host "✅ User registered successfully" -ForegroundColor Green
    }
    catch {
        Write-Host "ℹ️ User might already exist, continuing with login..." -ForegroundColor Yellow
    }

    # Login to get token
    $loginData = @{
        username = "weatheruser"
        password = "Weather123!"
    } | ConvertTo-Json

    $loginResponse = Invoke-RestMethod -Uri "$apiUrl/auth/login" -Method Post -Body $loginData -Headers $headers
    $token = $loginResponse.token
    Write-Host "✅ Login successful, got token" -ForegroundColor Green

    # Create weather forecast
    $createWeatherData = @{
        date = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
        temperatureC = 22
        summary = "Sunny"
        location = "Test City"
    } | ConvertTo-Json

    $authHeaders = @{
        "Content-Type" = "application/json"
        "Authorization" = "Bearer $token"
    }

    $newForecast = Invoke-RestMethod -Uri "$apiUrl/weather/" -Method Post -Body $createWeatherData -Headers $authHeaders
    Write-Host "✅ Weather forecast created successfully!" -ForegroundColor Green
    $newForecast | ConvertTo-Json

    # Test 6: Get the created forecast by ID
    Write-Host "`n6. Testing GET /weather/{id} (Get by ID)..." -ForegroundColor Cyan
    $forecastById = Invoke-RestMethod -Uri "$apiUrl/weather/$($newForecast.id)" -Method Get
    Write-Host "✅ Retrieved forecast by ID" -ForegroundColor Green
    $forecastById | ConvertTo-Json

    # Test 7: Update the forecast
    Write-Host "`n7. Testing PUT /weather/{id} (Update forecast)..." -ForegroundColor Cyan
    $updateWeatherData = @{
        date = (Get-Date).AddDays(1).ToString("yyyy-MM-dd")
        temperatureC = 25
        summary = "Very Sunny"
        location = "Updated Test City"
    } | ConvertTo-Json

    $updatedForecast = Invoke-RestMethod -Uri "$apiUrl/weather/$($newForecast.id)" -Method Put -Body $updateWeatherData -Headers $authHeaders
    Write-Host "✅ Weather forecast updated successfully!" -ForegroundColor Green
    $updatedForecast | ConvertTo-Json

    # Test 8: Delete the forecast
    Write-Host "`n8. Testing DELETE /weather/{id} (Delete forecast)..." -ForegroundColor Cyan
    Invoke-RestMethod -Uri "$apiUrl/weather/$($newForecast.id)" -Method Delete -Headers $authHeaders
    Write-Host "✅ Weather forecast deleted successfully!" -ForegroundColor Green

    # Test 9: Verify deletion
    Write-Host "`n9. Verifying deletion (should return 404)..." -ForegroundColor Cyan
    try {
        $deletedForecast = Invoke-RestMethod -Uri "$apiUrl/weather/$($newForecast.id)" -Method Get
        Write-Host "❌ Forecast still exists after deletion!" -ForegroundColor Red
    }
    catch {
        Write-Host "✅ Forecast successfully deleted (404 as expected)" -ForegroundColor Green
    }

}
catch {
    Write-Host "❌ Authentication or CRUD operations failed: $_" -ForegroundColor Red
    Write-Host "Error details: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== Weather Module Testing Complete ===" -ForegroundColor Green
