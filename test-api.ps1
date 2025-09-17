# MARA API Testing Script
# This PowerShell script demonstrates the JWT authentication flow and API usage

# Note: Replace {API_URL} with the actual API URL from Aspire dashboard
# Example: $apiUrl = "https://localhost:5001"
$apiUrl = "https://localhost:5001"  # Update this with your actual API URL

Write-Host "=== MARA API Testing Script ===" -ForegroundColor Green
Write-Host "API URL: $apiUrl" -ForegroundColor Yellow

# Test 1: Get API Info
Write-Host "`n1. Testing API Info endpoint..." -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$apiUrl/" -Method Get
    Write-Host "✅ API Info retrieved successfully" -ForegroundColor Green
    $response | ConvertTo-Json -Depth 3
}
catch {
    Write-Host "❌ Failed to get API info: $_" -ForegroundColor Red
}

# Test 2: Get Health Status
Write-Host "`n2. Testing Health Check..." -ForegroundColor Cyan
try {
    $health = Invoke-RestMethod -Uri "$apiUrl/health" -Method Get
    Write-Host "✅ Health check passed" -ForegroundColor Green
    $health | ConvertTo-Json
}
catch {
    Write-Host "❌ Health check failed: $_" -ForegroundColor Red
}

# Test 3: Login with demo user
Write-Host "`n3. Testing User Login..." -ForegroundColor Cyan
$loginData = @{
    username = "admin"
    password = "admin123"
} | ConvertTo-Json

$headers = @{
    "Content-Type" = "application/json"
}

try {
    $loginResponse = Invoke-RestMethod -Uri "$apiUrl/auth/login" -Method Post -Body $loginData -Headers $headers
    $token = $loginResponse.token
    Write-Host "✅ Login successful!" -ForegroundColor Green
    Write-Host "User: $($loginResponse.username)" -ForegroundColor Yellow
    Write-Host "Email: $($loginResponse.email)" -ForegroundColor Yellow
    Write-Host "Token: $($token.Substring(0, 20))..." -ForegroundColor Yellow
}
catch {
    Write-Host "❌ Login failed: $_" -ForegroundColor Red
    exit
}

# Test 4: Get User Profile
Write-Host "`n4. Testing Protected Endpoint - User Profile..." -ForegroundColor Cyan
$authHeaders = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

try {
    $profile = Invoke-RestMethod -Uri "$apiUrl/auth/profile" -Method Get -Headers $authHeaders
    Write-Host "✅ Profile retrieved successfully" -ForegroundColor Green
    $profile | ConvertTo-Json
}
catch {
    Write-Host "❌ Failed to get profile: $_" -ForegroundColor Red
}

# Test 5: Create a Todo
Write-Host "`n5. Testing Todo Creation..." -ForegroundColor Cyan
$todoData = @{
    title = "Test Todo from PowerShell"
    description = "This todo was created via API testing script"
} | ConvertTo-Json

try {
    $newTodo = Invoke-RestMethod -Uri "$apiUrl/todos" -Method Post -Body $todoData -Headers $authHeaders
    $todoId = $newTodo.id
    Write-Host "✅ Todo created successfully" -ForegroundColor Green
    Write-Host "Todo ID: $todoId" -ForegroundColor Yellow
    $newTodo | ConvertTo-Json
}
catch {
    Write-Host "❌ Failed to create todo: $_" -ForegroundColor Red
}

# Test 6: Get All Todos
Write-Host "`n6. Testing Get All Todos..." -ForegroundColor Cyan
try {
    $todos = Invoke-RestMethod -Uri "$apiUrl/todos" -Method Get -Headers $authHeaders
    Write-Host "✅ Todos retrieved successfully" -ForegroundColor Green
    Write-Host "Total todos: $($todos.Count)" -ForegroundColor Yellow
    $todos | ConvertTo-Json
}
catch {
    Write-Host "❌ Failed to get todos: $_" -ForegroundColor Red
}

# Test 7: Update Todo
if ($todoId) {
    Write-Host "`n7. Testing Todo Update..." -ForegroundColor Cyan
    $updateData = @{
        title = "Updated Todo from PowerShell"
        description = "This todo was updated via API testing script"
        isCompleted = $true
    } | ConvertTo-Json

    try {
        $updatedTodo = Invoke-RestMethod -Uri "$apiUrl/todos/$todoId" -Method Put -Body $updateData -Headers $authHeaders
        Write-Host "✅ Todo updated successfully" -ForegroundColor Green
        $updatedTodo | ConvertTo-Json
    }
    catch {
        Write-Host "❌ Failed to update todo: $_" -ForegroundColor Red
    }
}

# Test 8: Get Weather Forecast (Public endpoint)
Write-Host "`n8. Testing Weather Forecast (Public)..." -ForegroundColor Cyan
try {
    $weather = Invoke-RestMethod -Uri "$apiUrl/weather/forecast" -Method Get
    Write-Host "✅ Weather forecast retrieved successfully" -ForegroundColor Green
    Write-Host "Forecast days: $($weather.Count)" -ForegroundColor Yellow
    $weather | Select-Object Date, TemperatureC, TemperatureF, Summary | Format-Table
}
catch {
    Write-Host "❌ Failed to get weather forecast: $_" -ForegroundColor Red
}

# Test 9: Test Rate Limiting (optional)
Write-Host "`n9. Testing Rate Limiting..." -ForegroundColor Cyan
Write-Host "Sending multiple requests quickly to test rate limiting..." -ForegroundColor Yellow

for ($i = 1; $i -le 10; $i++) {
    try {
        $response = Invoke-RestMethod -Uri "$apiUrl/health" -Method Get -ErrorAction SilentlyContinue
        Write-Host "Request $i`: ✅ Success" -ForegroundColor Green
    }
    catch {
        if ($_.Exception.Response.StatusCode -eq 429) {
            Write-Host "Request $i`: ⚠️ Rate limited (429)" -ForegroundColor Yellow
        } else {
            Write-Host "Request $i`: ❌ Error: $_" -ForegroundColor Red
        }
    }
    Start-Sleep -Milliseconds 100
}

Write-Host "`n=== Testing Complete ===" -ForegroundColor Green
Write-Host "All major API endpoints have been tested!" -ForegroundColor Cyan
Write-Host "Check the Aspire dashboard for detailed telemetry and logs." -ForegroundColor Yellow
