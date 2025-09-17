using System.Security.Claims;
using mara.ApiService.Infrastructure.Models;

namespace mara.ApiService.Modules.Todos;

// Entity (now inherits from AuditableEntity)
public class Todo : AuditableEntity<int>
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int UserId { get; set; } // Foreign key to User
    
    // For Entity Framework compatibility 
    public DateTime UpdatedAt 
    { 
        get => ModifiedAt ?? CreatedAt; 
        set => ModifiedAt = value; 
    }
}

// Request/Response DTOs
public record CreateTodoRequest(string Title, string? Description);
public record UpdateTodoRequest(string Title, string? Description, bool IsCompleted);

// Service interface
public interface ITodoService
{
    Task<IEnumerable<Todo>> GetAllAsync(string userId, CancellationToken cancellationToken = default);
    Task<Todo?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default);
    Task<Todo> CreateAsync(CreateTodoRequest request, string userId, CancellationToken cancellationToken = default);
    Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request, string userId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default);
    
    // Todo-specific methods
    Task<IEnumerable<Todo>> GetCompletedTodosAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Todo>> GetPendingTodosAsync(string userId, CancellationToken cancellationToken = default);
}

// Service implementation (temporarily keeping simple implementation)
public class TodoService : ITodoService
{
    private readonly List<Todo> _todos = new();
    private static int _nextId = 1;

    public Task<IEnumerable<Todo>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        var userTodos = _todos.Where(t => t.CreatedBy == userId);
        return Task.FromResult(userTodos);
    }

    public Task<Todo?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id && t.CreatedBy == userId);
        return Task.FromResult(todo);
    }

    public Task<Todo> CreateAsync(CreateTodoRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var todo = new Todo
        {
            Id = Interlocked.Increment(ref _nextId),
            Title = request.Title,
            Description = request.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _todos.Add(todo);
        return Task.FromResult(todo);
    }

    public Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id && t.CreatedBy == userId);
        if (todo == null)
            return Task.FromResult<Todo?>(null);

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;

        return Task.FromResult<Todo?>(todo);
    }

    public Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id && t.CreatedBy == userId);
        if (todo == null)
            return Task.FromResult(false);

        _todos.Remove(todo);
        return Task.FromResult(true);
    }

    // Todo-specific methods
    public Task<IEnumerable<Todo>> GetCompletedTodosAsync(string userId, CancellationToken cancellationToken = default)
    {
        var completedTodos = _todos.Where(t => t.CreatedBy == userId && t.IsCompleted);
        return Task.FromResult(completedTodos);
    }

    public Task<IEnumerable<Todo>> GetPendingTodosAsync(string userId, CancellationToken cancellationToken = default)
    {
        var pendingTodos = _todos.Where(t => t.CreatedBy == userId && !t.IsCompleted);
        return Task.FromResult(pendingTodos);
    }
}

public class TodosModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register a simple in-memory implementation for now
        services.AddSingleton<ITodoService, TodoService>();
    }

    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var todosGroup = endpoints.MapGroup("/todos")
            .WithTags("Todos")
            .RequireAuthorization()
            .RequireRateLimiting("TodosPolicy");

        // Standard CRUD endpoints
        todosGroup.MapGet("/", async (ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var todos = await todoService.GetAllAsync(username);
            return Results.Ok(todos);
        }).WithName("GetAllTodos");

        todosGroup.MapGet("/{id:int}", async (int id, ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var todo = await todoService.GetByIdAsync(id, username);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        }).WithName("GetTodoById");

        todosGroup.MapPost("/", async (CreateTodoRequest request, ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var todo = await todoService.CreateAsync(request, username);
            return Results.Created($"/todos/{todo.Id}", todo);
        }).WithName("CreateTodo");

        todosGroup.MapPut("/{id:int}", async (int id, UpdateTodoRequest request, ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var todo = await todoService.UpdateAsync(id, request, username);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        }).WithName("UpdateTodo");

        todosGroup.MapDelete("/{id:int}", async (int id, ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var deleted = await todoService.DeleteAsync(id, username);
            return deleted ? Results.NoContent() : Results.NotFound();
        }).WithName("DeleteTodo");

        // Add custom Todo-specific endpoints
        todosGroup.MapGet("/completed", async (ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var todos = await todoService.GetCompletedTodosAsync(username);
            return Results.Ok(todos);
        })
        .WithName("GetCompletedTodos")
        .RequireAuthorization();

        todosGroup.MapGet("/pending", async (ClaimsPrincipal user, ITodoService todoService) =>
        {
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
                return Results.Unauthorized();

            var todos = await todoService.GetPendingTodosAsync(username);
            return Results.Ok(todos);
        })
        .WithName("GetPendingTodos")
        .RequireAuthorization();
    }
}
