# IMAS - Integrated Management and Administration System

A **production-ready microservices architecture** built with .NET 8 and ASP.NET Aspire, featuring modular design, JWT authentication, and complete separation between frontend and backend services.

## 🏗️ Architecture Overview

### Current Structure (After Reorganization)

```
MARA/
├── imas-FE/                          # 🎨 FRONTEND
│   └── imas.Web/                     # Blazor web application
│
├── imas-BE/                          # ⚙️ BACKEND
│   ├── imas.ApiService/              # Main API Gateway
│   ├── imas.HR.ApiService/           # HR Microservice
│   ├── imas.Assets.ApiService/       # Assets Microservice
│   ├── imas.Finance.ApiService/      # Finance Microservice
│   ├── imas.General.ApiService/      # General Microservice
│   ├── imas.AppHost/                 # .NET Aspire Orchestrator
│   ├── imas.ServiceDefaults/         # Shared service configuration
│   └── imas.Shared/                  # Minimal shared contracts
│
├── imas.sln                          # Solution file
├── docker-compose.*.yml              # Docker configurations
└── README.md                         # This file
```

## 🚀 Features

### 🎯 **Microservices Architecture**
- **Independent Services**: Each business domain runs as a separate API service
- **Database Isolation**: Each service has its own database
- **Independent Deployment**: Services can be deployed and scaled independently
- **Service Discovery**: Automatic endpoint resolution via .NET Aspire

### 🔐 **Security & Authentication**
- **JWT Token-based Authentication**: Secure authentication across all services
- **User Registration and Login**: Complete user management system
- **Role-based Authorization**: Admin and User roles
- **Password Hashing**: Secure BCrypt password hashing
- **Rate Limiting**: Configurable rate limits per endpoint group

### ⚡ **Performance & Reliability**
- **Rate Limiting Policies**:
  - Auth endpoints: 5 requests/minute (security)
  - Todo endpoints: 100 requests/minute (user operations)
  - Weather endpoints: 200 requests/minute (public data)
- **Async/Await Patterns**: Throughout all services
- **Cancellation Token Support**: For all operations
- **Health Checks**: Built-in health monitoring

### 🛠️ **Development Experience**
- **.NET Aspire Integration**: Modern development dashboard and orchestration
- **Hot Reload Support**: Instant changes without restart
- **Comprehensive Logging**: Distributed tracing and metrics
- **Swagger Documentation**: Interactive API documentation for all services

## 📋 Services Overview

### 1. **imas.ApiService** (Port 7000/5000) - Main API Gateway
- Routes requests to appropriate microservices
- Handles authentication and cross-cutting concerns
- Aggregates Swagger documentation
- **Endpoints**: `/auth`, `/todos`, `/weather`, `/health`

### 2. **imas.HR.ApiService** (Port 7005/5005) - Human Resources
- Employee management and directory
- Department organization
- Leave request management
- Payroll records tracking
- **Endpoints**: `/api/employees`, `/api/departments`, `/api/leave-requests`, `/api/payroll-records`

### 3. **imas.Assets.ApiService** (Port 7006/5006) - Asset Management
- Asset tracking and inventory
- Asset categories and classification
- Maintenance records and scheduling
- Asset assignments and transfers
- **Endpoints**: `/api/assets`, `/api/asset-categories`, `/api/asset-maintenance`, `/api/asset-assignments`

### 4. **imas.Finance.ApiService** (Port 7007/5007) - Financial Management
- Invoice management and processing
- Payment tracking and processing
- Budget planning and management
- Expense tracking and reporting
- **Endpoints**: `/api/invoices`, `/api/payments`, `/api/budgets`, `/api/expenses`

### 5. **imas.General.ApiService** (Port 7008/5008) - General Business
- Company management and profiles
- Customer relationship management
- Vendor management and contracts
- Document management system
- **Endpoints**: `/api/companies`, `/api/customers`, `/api/vendors`, `/api/documents`

### 6. **imas.Web** (Port 7100/5100) - Web Frontend
- Blazor Server application
- Responsive user interface
- Authentication integration
- Service communication

## 🚀 Getting Started

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** or **VS Code**
- **SQL Server** (LocalDB or full instance)
- **Docker** (optional, for containerized deployment)

### Quick Start with .NET Aspire (Recommended)

1. **Clone the repository**
   ```bash
   git clone [repository-url]
   cd MARA
   ```

2. **Start all services with one command**
   ```bash
   dotnet run --project imas-BE/imas.AppHost
   ```

3. **Access the Aspire Dashboard**
   - Navigate to the URL shown in console (typically `https://localhost:17238`)
   - View all services, logs, and metrics in one place

4. **Access Individual Services**
   - **API Gateway**: `https://localhost:7000/swagger`
   - **HR Service**: `https://localhost:7005/swagger`
   - **Assets Service**: `https://localhost:7006/swagger`
   - **Finance Service**: `https://localhost:7007/swagger`
   - **General Service**: `https://localhost:7008/swagger`
   - **Web Application**: `https://localhost:7100`

### Alternative: Individual Service Development

Run each service individually for development:

```bash
# Start each service in separate terminals
cd imas-BE/imas.HR.ApiService && dotnet run
cd imas-BE/imas.Assets.ApiService && dotnet run
cd imas-BE/imas.Finance.ApiService && dotnet run
cd imas-BE/imas.General.ApiService && dotnet run
cd imas-BE/imas.ApiService && dotnet run
cd imas-FE/imas.Web && dotnet run
```

### Docker Deployment

Run all services as containers:

```bash
docker-compose -f docker-compose.microservices.yml up --build
```

## 🧪 API Testing

### Demo Accounts
```
Username: admin, Password: admin123 (Admin role)
Username: user, Password: user123 (User role)
```

### Authentication Flow

1. **Register a new user**
   ```bash
   curl -X POST "https://localhost:7000/auth/register" \
     -H "Content-Type: application/json" \
     -d '{"username": "testuser", "email": "test@example.com", "password": "password123"}'
   ```

2. **Login to get JWT token**
   ```bash
   curl -X POST "https://localhost:7000/auth/login" \
     -H "Content-Type: application/json" \
     -d '{"username": "testuser", "password": "password123"}'
   ```

3. **Use token for protected endpoints**
   ```bash
   curl -X GET "https://localhost:7000/auth/profile" \
     -H "Authorization: Bearer {TOKEN}"
   ```

### Protected Operations

**Todo Management:**
```bash
# Create a todo
curl -X POST "https://localhost:7000/todos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TOKEN}" \
  -d '{"title": "Learn Aspire", "description": "Study ASP.NET Aspire framework"}'

# Get all todos
curl -X GET "https://localhost:7000/todos" \
  -H "Authorization: Bearer {TOKEN}"
```

**Public Endpoints:**
```bash
# Weather forecast
curl -X GET "https://localhost:7000/weather/forecast"

# Health check
curl -X GET "https://localhost:7000/health"
```

## 🗄️ Database Configuration

Each microservice has its own database for complete isolation:

- **ImasMainDb** - Main API Gateway data
- **ImasHRDb** - HR service data
- **ImasAssetsDb** - Assets service data
- **ImasFinanceDb** - Finance service data
- **ImasGeneralDb** - General service data

### Connection Strings

**Development (LocalDB):**
```
Server=(localdb)\\mssqllocaldb;Database={ServiceDb};Trusted_Connection=true;MultipleActiveResultSets=true
```

**Docker (SQL Server):**
```
Server=sqlserver;Database={ServiceDb};User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true
```

## 🔧 Configuration

### JWT Settings (`appsettings.json`)
```json
{
  "Jwt": {
    "SecretKey": "YourSecretKeyHere",
    "Issuer": "imas-api",
    "Audience": "imas-clients",
    "ExpirationMinutes": 60
  }
}
```

### Service Communication (`appsettings.json`)
```json
{
  "ModuleCommunication": {
    "Mode": "Distributed",
    "ServiceEndpoints": {
      "HR": "https://localhost:7005",
      "Assets": "https://localhost:7006",
      "Finance": "https://localhost:7007",
      "General": "https://localhost:7008"
    }
  }
}
```

## 🏗️ Generic CRUD Infrastructure

The system includes a comprehensive **Generic CRUD Base Infrastructure** that allows for rapid development of new modules with consistent patterns.

### Architecture Pattern

```
┌─────────────────────────┐
│   Module Endpoints      │  ← HTTP API Layer
├─────────────────────────┤
│   Service Layer         │  ← Business Logic
├─────────────────────────┤
│   Repository Layer      │  ← Data Access
├─────────────────────────┤
│   Entity/Model Layer    │  ← Data Models
└─────────────────────────┘
```

### Core Components

#### Base Entity Models
```csharp
public interface IEntity<TId>
{
    TId Id { get; set; }
}

public abstract class AuditableEntity<TId> : BaseEntity<TId>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}
```

#### Generic Repository Pattern
```csharp
public interface IRepository<TEntity, TId> 
    where TEntity : class, IEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
```

#### User-Scoped Services
```csharp
public interface IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TCreateRequest request, string userId, CancellationToken cancellationToken = default);
    // ... other CRUD methods
}
```

### Creating a New Module

1. **Define Entity**
   ```csharp
   public class Product : AuditableEntity<int>
   {
       public required string Name { get; set; }
       public decimal Price { get; set; }
   }
   ```

2. **Define DTOs**
   ```csharp
   public record CreateProductRequest(string Name, decimal Price);
   public record UpdateProductRequest(string Name, decimal Price);
   ```

3. **Implement Service**
   ```csharp
   public class ProductService : BaseUserScopedCrudService<Product, int, CreateProductRequest, UpdateProductRequest>
   {
       // Implementation with automatic CRUD operations
   }
   ```

4. **Create Module**
   ```csharp
   public class ProductsModule : IModule
   {
       public void RegisterServices(IServiceCollection services, IConfiguration configuration)
       {
           services.AddScoped<IProductService, ProductService>();
       }

       public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
       {
           var group = endpoints.MapGroup("/products").RequireAuthorization();
           group.MapUserScopedCrudEndpoints<Product, int, CreateProductRequest, UpdateProductRequest>("Product");
       }
   }
   ```

### Standard Endpoints Generated
Each module automatically gets:
- `GET /entities` - Get all entities for the user
- `GET /entities/{id}` - Get specific entity by ID
- `POST /entities` - Create new entity
- `PUT /entities/{id}` - Update existing entity
- `DELETE /entities/{id}` - Delete entity

## 📊 .NET Aspire Features

### 🔍 **Service Discovery**
- Automatic service registration and discovery
- No manual endpoint configuration needed
- Dynamic port allocation

### ❤️ **Health Checks**
- Built-in health monitoring for all services
- Detailed health status reporting
- Dependency health tracking

### 📈 **Telemetry & Monitoring**
- Distributed tracing across services
- Metrics collection and aggregation
- Real-time performance monitoring
- Centralized logging

### 🎛️ **Development Dashboard**
- Visual service overview
- Real-time logs from all services
- Service dependency visualization
- Resource usage monitoring

### ⚙️ **Configuration Management**
- Centralized configuration injection
- Environment-specific settings
- Automatic connection string management

## 🐳 Deployment Options

### 1. **Development** - .NET Aspire (Recommended)
```bash
dotnet run --project imas-BE/imas.AppHost
```
- **Benefits**: Fastest startup, full debugging, hot reload
- **Use for**: Local development, debugging, rapid iteration

### 2. **Containerized Development** - Docker Compose
```bash
docker-compose -f docker-compose.microservices.yml up --build
```
- **Benefits**: Production-like environment, service isolation
- **Use for**: Integration testing, team environments

### 3. **Production** - Kubernetes/Cloud
- **Kubernetes manifests** (can be generated)
- **Azure Container Instances**
- **AWS ECS**
- **Google Cloud Run**

## 📚 Technology Stack

### Backend Technologies
- **.NET 8** - Latest .NET framework
- **ASP.NET Aspire 8.2+** - Cloud-ready application orchestration
- **Minimal APIs** - Lightweight, fast API endpoints
- **Entity Framework Core** - Data access and ORM
- **JWT Bearer Authentication** - Secure token-based authentication
- **BCrypt.Net** - Password hashing and security
- **Swagger/OpenAPI** - API documentation and testing

### Frontend Technologies
- **Blazor Server** - Modern web UI framework
- **Bootstrap** - Responsive CSS framework
- **SignalR** - Real-time communication

### Infrastructure
- **SQL Server** - Database engine
- **Docker** - Containerization
- **Docker Compose** - Multi-container orchestration

## 🔒 Security Features

### Authentication & Authorization
- **JWT token validation** with configurable expiration
- **Role-based authorization** (Admin, User)
- **Password hashing** using BCrypt
- **User-scoped data access** - Users can only access their own data

### Rate Limiting
- **Authentication endpoints**: 5 requests/minute (security)
- **Business endpoints**: 100 requests/minute (normal operations)
- **Public endpoints**: 200 requests/minute (weather, health checks)

### Data Protection
- **User isolation** at data layer
- **Audit trails** with created/updated timestamps
- **Secure password storage**
- **SQL injection prevention** via Entity Framework

## 🧪 Testing Strategy

### Unit Testing
- **Service layer testing** with mocked dependencies
- **Repository pattern testing** with in-memory databases
- **Business logic validation**

### Integration Testing
- **API endpoint testing** with WebApplicationFactory
- **Database integration testing**
- **Authentication flow testing**

### End-to-End Testing
- **Multi-service communication testing**
- **User workflow testing**
- **Cross-service transaction testing**

## 🚀 Performance Features

### Optimization
- **Async/await patterns** throughout
- **Cancellation token support** for all operations
- **Efficient in-memory repositories** with LINQ expressions
- **Connection pooling** and database optimization

### Scalability
- **Horizontal scaling** of individual services
- **Database isolation** for independent scaling
- **Stateless service design**
- **Load balancing ready**

## 📈 Monitoring & Observability

### Logging
- **Structured logging** with Serilog
- **Distributed tracing** across services
- **Correlation IDs** for request tracking
- **Centralized log aggregation**

### Metrics
- **Application metrics** via .NET Aspire
- **Custom business metrics**
- **Performance counters**
- **Health check metrics**

### Alerting
- **Health check failures**
- **Performance degradation**
- **Error rate thresholds**
- **Resource utilization**

## 🛠️ Development Guidelines

### Code Organization
- **Domain-driven design** principles
- **Clean architecture** with clear separation of concerns
- **SOLID principles** throughout
- **Dependency injection** for all services

### Best Practices
- **Consistent error handling** and validation
- **Comprehensive documentation** via XML comments
- **Type safety** with strong typing throughout
- **Testable design** with interface abstractions

### Module Development
1. **Define contracts** in `imas.Shared` if cross-service communication needed
2. **Implement business logic** in respective service
3. **Add comprehensive tests** for all functionality
4. **Update documentation** for new endpoints
5. **Add health checks** for dependencies

## 🔮 Roadmap & Extensions

### Near-term Enhancements
- **Email verification** for user registration
- **Password reset** functionality
- **Refresh tokens** for enhanced security
- **File upload** capabilities
- **Advanced search** and filtering

### Advanced Features
- **Event-driven architecture** with message queues
- **CQRS pattern** for complex operations
- **Event sourcing** for audit trails
- **Background job processing**
- **Real-time notifications**

### DevOps & Quality
- **CI/CD pipelines** with GitHub Actions
- **Automated testing** in pipelines
- **Code quality gates** with SonarQube
- **Security scanning** with tools like Snyk
- **Performance testing** with NBomber

### Cloud & Deployment
- **Kubernetes Helm charts**
- **Azure DevOps integration**
- **Application Insights** monitoring
- **Azure Key Vault** for secrets
- **Auto-scaling** configurations

## 📝 Migration Notes

### From Monolith to Microservices
The project has been successfully transformed from a modular monolith to a true microservices architecture:

- ✅ **Service Separation** - Each module runs as independent API service
- ✅ **Database Isolation** - Each service has its own database
- ✅ **Independent Deployment** - Services can be deployed separately
- ✅ **Container Support** - Full Docker containerization
- ✅ **Frontend/Backend Separation** - Clean separation into `imas-FE` and `imas-BE`

### Service Independence Status
- **HR Service**: ✅ Fully independent
- **Assets Service**: ✅ Fully independent
- **Finance Service**: ✅ Fully independent
- **General Service**: ✅ Fully independent
- **API Gateway**: ✅ Orchestrates all services
- **Web Frontend**: ✅ Separated and communicates via API Gateway

## 🤝 Contributing

### Development Setup
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Update documentation
7. Submit a pull request

### Code Standards
- Follow **C# coding conventions**
- Write **comprehensive unit tests**
- Include **XML documentation** for public APIs
- Use **meaningful commit messages**
- Update **README** for new features

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support & Contact

For questions, issues, or contributions:
- **Create an issue** in the GitHub repository
- **Join discussions** in GitHub Discussions
- **Review documentation** in the `/docs` folder

---

**Built with ❤️ using .NET 8 and ASP.NET Aspire**

*This project demonstrates modern .NET microservices architecture with production-ready patterns, comprehensive testing, and excellent developer experience.*