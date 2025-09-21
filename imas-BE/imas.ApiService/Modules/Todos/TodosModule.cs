using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using imas.ApiService.Infrastructure.Models;
using imas.ApiService.Infrastructure.Data;

namespace imas.ApiService.Modules.Todos;

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

// Request/Response DTOs (Legacy - to be migrated later)
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

// Database-backed TodoService implementation using SharedImasDbContext
public class TodoService : ITodoService
{
    private readonly SharedImasDbContext _context;

    public TodoService(SharedImasDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Todo>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .Where(t => t.CreatedBy == userId && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Todo?> GetByIdAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.CreatedBy == userId && !t.IsDeleted, cancellationToken);
    }

    public async Task<Todo> CreateAsync(CreateTodoRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description,
            IsCompleted = false,
            UserId = 0, // We'll need to resolve this from the user
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(cancellationToken);
        return todo;
    }

    public async Task<Todo?> UpdateAsync(int id, UpdateTodoRequest request, string userId, CancellationToken cancellationToken = default)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.CreatedBy == userId && !t.IsDeleted, cancellationToken);
        
        if (todo == null)
            return null;

        todo.Title = request.Title;
        todo.Description = request.Description;
        todo.IsCompleted = request.IsCompleted;
        todo.ModifiedAt = DateTime.UtcNow;
        todo.ModifiedBy = userId;

        await _context.SaveChangesAsync(cancellationToken);
        return todo;
    }

    public async Task<bool> DeleteAsync(int id, string userId, CancellationToken cancellationToken = default)
    {
        var todo = await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id && t.CreatedBy == userId && !t.IsDeleted, cancellationToken);
        
        if (todo == null)
            return false;

        todo.IsDeleted = true;
        todo.DeletedAt = DateTime.UtcNow;
        todo.DeletedBy = userId;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    // Todo-specific methods
    public async Task<IEnumerable<Todo>> GetCompletedTodosAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .Where(t => t.CreatedBy == userId && t.IsCompleted && !t.IsDeleted)
            .OrderByDescending(t => t.ModifiedAt ?? t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Todo>> GetPendingTodosAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .Where(t => t.CreatedBy == userId && !t.IsCompleted && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

public class TodosModule : IModule
{
    public void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        // Register database-backed implementation
        services.AddScoped<ITodoService, TodoService>();
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
