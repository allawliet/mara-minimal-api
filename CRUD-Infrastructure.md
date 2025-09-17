# MARA API - Generic CRUD Base Infrastructure

This document explains the generic CRUD base functionality that has been implemented in the MARA API project. This infrastructure allows for rapid development of new modules with consistent patterns.

## 🏗️ **Architecture Overview**

The CRUD base infrastructure follows a layered architecture pattern:

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

## 📁 **Project Structure**

```
mara.ApiService/
├── Infrastructure/
│   ├── Models/
│   │   ├── BaseEntity.cs              # Base entity interface & classes
│   │   └── AuditableEntity.cs         # Entity with audit fields
│   ├── Repository/
│   │   ├── IRepository.cs             # Generic repository interface
│   │   ├── InMemoryRepository.cs      # Generic in-memory repository
│   │   └── UserScopedInMemoryRepository.cs # User-scoped repository
│   ├── Services/
│   │   └── BaseCrudService.cs         # Generic CRUD service base
│   └── Endpoints/
│       └── CrudEndpoints.cs           # Generic endpoint mappings
└── Modules/
    ├── Todos/
    │   └── TodosModule.cs             # Example implementation
    └── Weather/
        └── WeatherModule.cs           # Another module
```

## 🔧 **Core Components**

### 1. **Base Entity Models**

#### `IEntity<TId>` Interface
```csharp
public interface IEntity<TId>
{
    TId Id { get; set; }
}
```

#### `BaseEntity<TId>` Class
```csharp
public abstract class BaseEntity<TId> : IEntity<TId>
{
    public virtual TId Id { get; set; } = default!;
}
```

#### `AuditableEntity<TId>` Class
```csharp
public abstract class AuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public string? UpdatedBy { get; set; }
}
```

### 2. **Repository Pattern**

#### Generic Repository Interface
```csharp
public interface IRepository<TEntity, TId> 
    where TEntity : class, IEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> SearchAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
}
```

#### User-Scoped Repository
```csharp
public interface IUserScopedRepository<TEntity, TId> 
    where TEntity : class, IEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, string userId, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, string userId, CancellationToken cancellationToken = default);
}
```

### 3. **Service Layer**

#### Generic CRUD Service
```csharp
public interface ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TCreateRequest request, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TId id, TUpdateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, CancellationToken cancellationToken = default);
}
```

#### User-Scoped CRUD Service
```csharp
public interface IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest>
    where TEntity : class, IEntity<TId>
{
    Task<IEnumerable<TEntity>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TId id, string userId, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TCreateRequest request, string userId, CancellationToken cancellationToken = default);
    Task<TEntity?> UpdateAsync(TId id, TUpdateRequest request, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TId id, string userId, CancellationToken cancellationToken = default);
}
```

### 4. **Generic Endpoints**

The `CrudEndpoints` class provides extension methods to automatically map standard CRUD endpoints:

```csharp
public static RouteGroupBuilder MapCrudEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest>(
    this RouteGroupBuilder group,
    ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> service,
    string entityName)
    where TEntity : class, IEntity<TId>

public static RouteGroupBuilder MapUserScopedCrudEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest>(
    this RouteGroupBuilder group,
    IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> service,
    string entityName)
    where TEntity : class, IEntity<TId>
```

## 🚀 **How to Create a New Module**

### Step 1: Define Your Entity
```csharp
public class Product : AuditableEntity<int>
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
```

### Step 2: Define Request/Response DTOs
```csharp
public record CreateProductRequest(string Name, string? Description, decimal Price, int CategoryId);
public record UpdateProductRequest(string Name, string? Description, decimal Price, int CategoryId);
```

### Step 3: Create Service Interface (Optional)
```csharp
public interface IProductService : IUserScopedCrudService<Product, int, CreateProductRequest, UpdateProductRequest>
{
    // Add custom methods if needed
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, string userId, CancellationToken cancellationToken = default);
}
```

### Step 4: Implement Service
```csharp
public class ProductService : BaseUserScopedCrudService<Product, int, CreateProductRequest, UpdateProductRequest>, IProductService
{
    public ProductService(IUserScopedRepository<Product, int> repository) : base(repository) { }

    protected override Task<Product> MapCreateRequestToEntityAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Id = GenerateId(), // Implement your ID generation logic
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            CategoryId = request.CategoryId
        };
        return Task.FromResult(product);
    }

    protected override Task MapUpdateRequestToEntityAsync(UpdateProductRequest request, Product entity, CancellationToken cancellationToken = default)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.CategoryId = request.CategoryId;
        return Task.CompletedTask;
    }

    // Custom methods
    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId, string userId, CancellationToken cancellationToken = default)
    {
        return SearchAsync(userId, p => p.CategoryId == categoryId, cancellationToken);
    }
}
```

### Step 5: Create Module
```csharp
public class ProductsModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUserScopedRepository<Product, int>, UserScopedInMemoryRepository<Product, int>>();
        services.AddScoped<IProductService, ProductService>();
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var productsGroup = endpoints.MapGroup("/products")
            .WithTags("Products")
            .RequireAuthorization()
            .RequireRateLimiting("ProductsPolicy");

        // Use generic CRUD endpoints
        productsGroup.MapUserScopedCrudEndpoints<Product, int, CreateProductRequest, UpdateProductRequest>("Product");

        // Add custom endpoints
        productsGroup.MapGet("/category/{categoryId:int}", async (int categoryId, ClaimsPrincipal user, IProductService productService) =>
        {
            var userId = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var products = await productService.GetProductsByCategoryAsync(categoryId, userId);
            return Results.Ok(products);
        })
        .WithName("GetProductsByCategory")
        .RequireAuthorization();
    }
}
```

### Step 6: Register Module
Add your module to `ModuleExtensions.cs`:
```csharp
private static List<IModule> GetModules()
{
    return new List<IModule>
    {
        new AuthenticationModule(),
        new TodosModule(),
        new WeatherModule(),
        new ProductsModule() // Add your new module
    };
}
```

## 📊 **Features Provided by Base Infrastructure**

### 🔒 **Security Features**
- **User-scoped data access** - Users can only access their own data
- **Authorization integration** - Works with JWT authentication
- **Audit trails** - Automatic tracking of created/updated timestamps and users

### 🚀 **Performance Features**
- **Async/await patterns** throughout
- **Cancellation token support** for all operations
- **Efficient in-memory repositories** with LINQ expressions
- **Pagination support** for large datasets

### 📋 **Standard Endpoints**
Each module automatically gets:
- `GET /entities` - Get all entities for the user
- `GET /entities/{id}` - Get specific entity by ID
- `POST /entities` - Create new entity
- `PUT /entities/{id}` - Update existing entity
- `DELETE /entities/{id}` - Delete entity
- `GET /entities/paged` - Paginated results

### 🔍 **Search & Filtering**
- **Expression-based filtering** using LINQ
- **Custom search methods** in services
- **Flexible predicate support**

### 🛡️ **Error Handling**
- **Consistent error responses**
- **Validation support**
- **Proper HTTP status codes**

## 🎯 **Benefits**

1. **Rapid Development** - New modules can be created in minutes
2. **Consistency** - All modules follow the same patterns
3. **Maintainability** - Centralized business logic and patterns
4. **Testability** - Clear separation of concerns
5. **Extensibility** - Easy to add new features and customize behavior
6. **Type Safety** - Strong typing throughout all layers
7. **Security** - Built-in user isolation and authorization

## 📚 **Example: Todo Module Implementation**

The `TodosModule` demonstrates how to use the base infrastructure:

```csharp
// Entity inherits from AuditableEntity
public class Todo : AuditableEntity<int>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
}

// DTOs for API
public record CreateTodoRequest(string Title, string? Description);
public record UpdateTodoRequest(string Title, string? Description, bool IsCompleted);

// Service with custom methods
public interface ITodoService
{
    // Standard CRUD methods
    Task<IEnumerable<Todo>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<Todo?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Todo> CreateAsync(CreateTodoRequest request, string userId, CancellationToken cancellationToken = default);
    Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    
    // Custom methods
    Task<IEnumerable<Todo>> GetCompletedTodosAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Todo>> GetPendingTodosAsync(string userId, CancellationToken cancellationToken = default);
}
```

This base infrastructure makes it extremely easy to create new modules while maintaining consistency, security, and best practices across the entire API! 🎉
