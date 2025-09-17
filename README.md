# MARA - Minimal API with ASP.NET Aspire

This project demonstrates a **production-ready minimal API** built with ASP.NET Aspire, featuring **JWT authentication**, **rate limiting**, and **modular architecture**.

## Project Structure

```
mara/
├── mara.AppHost/                    # Aspire orchestration host
├── mara.ApiService/                 # Minimal API service
│   ├── Configuration/               # App configuration models
│   │   └── JwtOptions.cs
│   ├── Modules/                     # Feature modules
│   │   ├── IModule.cs              # Module interface
│   │   ├── ModuleExtensions.cs     # Module registration
│   │   ├── Authentication/         # Auth module
│   │   ├── Todos/                  # Todo CRUD module
│   │   └── Weather/                # Weather API module
│   └── Program.cs                   # Application startup
├── mara.ServiceDefaults/            # Shared service configuration
├── mara.Web/                       # Web frontend (Blazor)
└── mara.sln                        # Solution file
```

## 🚀 Features

### 🔐 JWT Authentication
- **Secure JWT token-based authentication**
- **User registration and login endpoints**
- **Role-based authorization** (Admin, User)
- **Password hashing** with BCrypt
- **Token validation middleware**

### ⚡ Rate Limiting
- **Configurable rate limiting policies**
- **Different limits per endpoint group**:
  - **Auth endpoints**: 5 requests/minute (security)
  - **Todo endpoints**: 100 requests/minute (user operations)
  - **Weather endpoints**: 200 requests/minute (public data)

### 📦 Modular Architecture
- **Clean separation of concerns**
- **Module-based organization**
- **Dependency injection per module**
- **Easy to extend and maintain**

### API Endpoints

#### 🏠 General
- `GET /` - API information and available endpoints
- `GET /health` - Application health status with detailed info

#### 🔑 Authentication (`/auth`)
- `POST /auth/login` - User login (returns JWT token)
- `POST /auth/register` - User registration
- `GET /auth/profile` - Get user profile (requires auth)

#### 📝 Todo Management (`/todos`) - *Requires Authentication*
- `GET /todos` - Get user's todos
- `GET /todos/{id}` - Get specific todo by ID
- `POST /todos` - Create a new todo
- `PUT /todos/{id}` - Update an existing todo
- `DELETE /todos/{id}` - Delete a todo

#### 🌤️ Weather API (`/weather`) - *Public*
- `GET /weather/forecast` - Get 5-day weather forecast
- `GET /weather/forecast/{days}` - Get custom weather forecast (1-30 days)

### 📚 Swagger/OpenAPI Support
- **Complete API documentation**
- **JWT Bearer authentication in Swagger UI**
- **Interactive API testing**
- **Available at `/swagger` in development**

## 🛠️ Getting Started

### Prerequisites
- **.NET 8.0 SDK**
- **Visual Studio 2022** or **VS Code**

### Running the Application

1. **Clone the repository**
2. **Navigate to the project directory**
3. **Run the application**:
   ```bash
   dotnet run --project mara.AppHost
   ```

4. **Open the Aspire dashboard** at the URL shown in the console (typically `https://localhost:17238`)

### 🧪 API Testing

#### 1. Using Swagger UI
- Navigate to the **API service URL** + `/swagger`
- Use the **"Authorize"** button to enter your JWT token
- Test all endpoints interactively

#### 2. Using cURL

**Authentication Flow:**
```bash
# Register a new user
curl -X POST "https://localhost:{api-port}/auth/register" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "email": "test@example.com", "password": "password123"}'

# Login to get JWT token
curl -X POST "https://localhost:{api-port}/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"username": "testuser", "password": "password123"}'

# Use demo accounts:
# Username: admin, Password: admin123 (Admin role)
# Username: user, Password: user123 (User role)
```

**Protected Todo Operations:**
```bash
# Get user profile (replace {TOKEN} with actual JWT)
curl -X GET "https://localhost:{api-port}/auth/profile" \
  -H "Authorization: Bearer {TOKEN}"

# Create a todo
curl -X POST "https://localhost:{api-port}/todos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TOKEN}" \
  -d '{"title": "Learn Aspire", "description": "Study ASP.NET Aspire framework"}'

# Get all todos
curl -X GET "https://localhost:{api-port}/todos" \
  -H "Authorization: Bearer {TOKEN}"
```

**Public Endpoints:**
```bash
# Get weather forecast
curl -X GET "https://localhost:{api-port}/weather/forecast"

# Get custom weather forecast
curl -X GET "https://localhost:{api-port}/weather/forecast/10"

# Health check
curl -X GET "https://localhost:{api-port}/health"

# API info
curl -X GET "https://localhost:{api-port}/"
```

## 🏗️ Technology Stack

- **ASP.NET Aspire 8.2+** - Cloud-ready application orchestration
- **JWT Bearer Authentication** - Secure token-based auth
- **Rate Limiting** - Built-in .NET 8 rate limiting
- **BCrypt.Net** - Password hashing
- **Swagger/OpenAPI** - API documentation and testing
- **Minimal APIs** - Lightweight, fast API endpoints
- **Blazor** - Modern web UI framework
- **.NET 8** - Latest .NET framework

## ✨ Aspire Features Used

- **🔍 Service Discovery** - Automatic service registration and discovery
- **❤️ Health Checks** - Built-in health monitoring with detailed status
- **📊 Telemetry** - Distributed tracing and metrics collection
- **⚙️ Configuration Management** - Centralized configuration
- **📱 Development Dashboard** - Real-time application monitoring
- **🐳 Container Orchestration** - Docker container management

## 🔧 Configuration

### JWT Settings (`appsettings.json`)
```json
{
  "Jwt": {
    "SecretKey": "YourSecretKeyHere",
    "Issuer": "mara-api",
    "Audience": "mara-clients",
    "ExpirationMinutes": 60
  }
}
```

### Rate Limiting Policies
- **Authentication**: Fixed window, 5 requests/minute
- **Todos**: Token bucket, 100 requests/minute
- **Weather**: Sliding window, 200 requests/minute

## 💡 Architecture Highlights

### Modular Design
- **Self-contained modules** with their own services and endpoints
- **Clean separation** of authentication, business logic, and data access
- **Easy to test** and maintain individual components

### Security Features
- **JWT token validation** with configurable expiration
- **Password hashing** using BCrypt
- **Role-based authorization** support
- **Rate limiting** to prevent abuse

### Production Ready
- **Proper error handling** and validation
- **Comprehensive logging** via Aspire telemetry
- **Health checks** for monitoring
- **API documentation** with Swagger

## 🚀 Next Steps

To extend this project, consider:

### Database Integration
- **Entity Framework Core** with SQL Server/PostgreSQL
- **Database migrations** and seeding
- **Repository pattern** implementation

### Advanced Features
- **Email verification** for user registration
- **Password reset** functionality
- **Refresh tokens** for enhanced security
- **Role management** endpoints
- **File upload** capabilities

### Testing & Quality
- **Unit tests** with xUnit
- **Integration tests** with WebApplicationFactory
- **Load testing** with NBomber
- **Static code analysis** with SonarQube

### DevOps & Deployment
- **Docker containerization** 
- **Kubernetes deployment**
- **CI/CD pipelines** with GitHub Actions
- **Application monitoring** with Application Insights
