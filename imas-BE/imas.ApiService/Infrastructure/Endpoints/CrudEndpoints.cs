using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using imas.ApiService.Infrastructure.Models;
using imas.ApiService.Infrastructure.Services;

namespace imas.ApiService.Infrastructure.Endpoints;

/// <summary>
/// Generic CRUD endpoints that can be used by any module
/// </summary>
public static class CrudEndpoints
{
    /// <summary>
/// Map standard CRUD endpoints for a resource
/// </summary>
public static RouteGroupBuilder MapCrudEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest>(
    this RouteGroupBuilder group,
    ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> service,
    string entityName)
    where TEntity : class, IEntity<TId>
{
    // GET all
    group.MapGet("/", async (ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var items = await svc.GetAllAsync(ct);
        return Results.Ok(items);
    })
    .WithName($"GetAll{entityName}")
    .WithSummary($"Get all {entityName.ToLower()} items")
    .Produces<IEnumerable<TEntity>>();

    // GET by ID
    group.MapGet("/{id}", async (TId id, ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var item = await svc.GetByIdAsync(id, ct);
        return item != null ? Results.Ok(item) : Results.NotFound();
    })
    .WithName($"Get{entityName}ById")
    .WithSummary($"Get {entityName.ToLower()} by ID")
    .Produces<TEntity>()
    .Produces(404);

    // POST create
    group.MapPost("/", async (TCreateRequest request, ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var item = await svc.CreateAsync(request, ct);
        return Results.Created($"/{entityName.ToLower()}/{item.Id}", item);
    })
    .WithName($"Create{entityName}")
    .WithSummary($"Create a new {entityName.ToLower()}")
    .Produces<TEntity>(201)
    .Produces(400);

    // PUT update
    group.MapPut("/{id}", async (TId id, TUpdateRequest request, ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var item = await svc.UpdateAsync(id, request, ct);
        return item != null ? Results.Ok(item) : Results.NotFound();
    })
    .WithName($"Update{entityName}")
    .WithSummary($"Update {entityName.ToLower()} by ID")
    .Produces<TEntity>()
    .Produces(404)
    .Produces(400);

    // DELETE
    group.MapDelete("/{id}", async (TId id, ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var deleted = await svc.DeleteAsync(id, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    })
    .WithName($"Delete{entityName}")
    .WithSummary($"Delete {entityName.ToLower()} by ID")
    .Produces(204)
    .Produces(404);

    return group;
}

/// <summary>
/// Map user-scoped CRUD endpoints for a resource
/// </summary>
public static RouteGroupBuilder MapUserScopedCrudEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest>(
    this RouteGroupBuilder group,
    IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> service,
    string entityName)
    where TEntity : class, IEntity<TId>, ICreatedEntity
{
    // GET all (user-scoped)
    group.MapGet("/", async (ClaimsPrincipal user, IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var userId = GetUserId(user);
        if (userId == null) return Results.Unauthorized();

        var items = await svc.GetAllAsync(userId, ct);
        return Results.Ok(items);
    })
    .WithName($"GetAll{entityName}")
    .WithSummary($"Get all user's {entityName.ToLower()} items")
    .Produces<IEnumerable<TEntity>>()
    .Produces(401);

    // GET by ID (user-scoped)
    group.MapGet("/{id}", async (TId id, ClaimsPrincipal user, IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var userId = GetUserId(user);
        if (userId == null) return Results.Unauthorized();

        var item = await svc.GetByIdAsync(id, userId, ct);
        return item != null ? Results.Ok(item) : Results.NotFound();
    })
    .WithName($"Get{entityName}ById")
    .WithSummary($"Get user's {entityName.ToLower()} by ID")
    .Produces<TEntity>()
    .Produces(404)
    .Produces(401);

    // POST create (user-scoped)
    group.MapPost("/", async (TCreateRequest request, ClaimsPrincipal user, IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var userId = GetUserId(user);
        if (userId == null) return Results.Unauthorized();

        var item = await svc.CreateAsync(request, userId, ct);
        return Results.Created($"/{entityName.ToLower()}/{item.Id}", item);
    })
    .WithName($"Create{entityName}")
    .WithSummary($"Create a new {entityName.ToLower()}")
    .Produces<TEntity>(201)
    .Produces(400)
    .Produces(401);

    // PUT update (user-scoped)
    group.MapPut("/{id}", async (TId id, TUpdateRequest request, ClaimsPrincipal user, IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var userId = GetUserId(user);
        if (userId == null) return Results.Unauthorized();

        var item = await svc.UpdateAsync(id, request, userId, ct);
        return item != null ? Results.Ok(item) : Results.NotFound();
    })
    .WithName($"Update{entityName}")
    .WithSummary($"Update user's {entityName.ToLower()} by ID")
    .Produces<TEntity>()
    .Produces(404)
    .Produces(400)
    .Produces(401);

    // DELETE (user-scoped)
    group.MapDelete("/{id}", async (TId id, ClaimsPrincipal user, IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, CancellationToken ct) =>
    {
        var userId = GetUserId(user);
        if (userId == null) return Results.Unauthorized();

        var deleted = await svc.DeleteAsync(id, userId, ct);
        return deleted ? Results.NoContent() : Results.NotFound();
    })
    .WithName($"Delete{entityName}")
    .WithSummary($"Delete user's {entityName.ToLower()} by ID")
    .Produces(204)
    .Produces(404)
    .Produces(401);

    return group;
}

/// <summary>
/// Map paginated endpoints for a resource
/// </summary>
public static RouteGroupBuilder MapPaginatedEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest>(
    this RouteGroupBuilder group,
    ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> service,
    string entityName)
    where TEntity : class, IEntity<TId>
{
    group.MapGet("/paged", async (ICrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken ct = default) =>
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, totalCount) = await svc.GetPagedAsync(page, pageSize, ct);
        
        var response = new
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        return Results.Ok(response);
    })
    .WithName($"GetPaged{entityName}")
    .WithSummary($"Get paginated {entityName.ToLower()} items");

    return group;
}

/// <summary>
/// Map user-scoped paginated endpoints for a resource
/// </summary>
public static RouteGroupBuilder MapUserScopedPaginatedEndpoints<TEntity, TId, TCreateRequest, TUpdateRequest>(
    this RouteGroupBuilder group,
    IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> service,
    string entityName)
    where TEntity : class, IEntity<TId>, ICreatedEntity
{
    group.MapGet("/paged", async ([FromQuery] int page = 1, [FromQuery] int pageSize = 10, ClaimsPrincipal user = default!, IUserScopedCrudService<TEntity, TId, TCreateRequest, TUpdateRequest> svc = default!, CancellationToken ct = default) =>
    {
        var userId = GetUserId(user);
        if (userId == null) return Results.Unauthorized();

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var (items, totalCount) = await svc.GetPagedAsync(userId, page, pageSize, ct);
        
        var response = new
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
        };

        return Results.Ok(response);
    })
    .WithName($"GetPaged{entityName}")
    .WithSummary($"Get paginated user {entityName.ToLower()} items");

    return group;
}

private static string? GetUserId(ClaimsPrincipal user)
{
    return user.FindFirst(ClaimTypes.Name)?.Value;
}
}
