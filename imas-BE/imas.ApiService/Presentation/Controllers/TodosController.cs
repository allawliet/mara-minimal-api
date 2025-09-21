using Microsoft.AspNetCore.Mvc;
using imas.ApiService.Application.Common;
using imas.ApiService.Application.Todos.Commands;
using imas.ApiService.Application.Todos.DTOs;
using imas.ApiService.Application.Todos.Queries;
using imas.ApiService.Domain.Common;

namespace imas.ApiService.Presentation.Controllers;

/// <summary>
/// API Controller for Todo operations following Clean Architecture
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TodosController : ControllerBase
{
    private readonly ILogger<TodosController> _logger;

    public TodosController(ILogger<TodosController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Get all todos for the current user
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos(
        [FromServices] IRequestHandler<GetAllTodosQuery, Result<IEnumerable<TodoDto>>> handler,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1"); // Temporary hardcoded value
        
        var query = new GetAllTodosQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get todos with pagination
    /// </summary>
    [HttpGet("paged")]
    public async Task<ActionResult<PagedResult<TodoDto>>> GetPagedTodos(
        [FromServices] IRequestHandler<GetPagedTodosQuery, Result<PagedResult<TodoDto>>> handler,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var query = new GetPagedTodosQuery(userId, page, pageSize);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a specific todo by ID
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TodoDto>> GetTodo(
        [FromServices] IRequestHandler<GetTodoByIdQuery, Result<TodoDto>> handler,
        int id,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var query = new GetTodoByIdQuery(id, userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Todo not found")
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get completed todos
    /// </summary>
    [HttpGet("completed")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetCompletedTodos(
        [FromServices] IRequestHandler<GetCompletedTodosQuery, Result<IEnumerable<TodoDto>>> handler,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var query = new GetCompletedTodosQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get pending todos
    /// </summary>
    [HttpGet("pending")]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetPendingTodos(
        [FromServices] IRequestHandler<GetPendingTodosQuery, Result<IEnumerable<TodoDto>>> handler,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var query = new GetPendingTodosQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get todo statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<TodoStatsDto>> GetTodoStats(
        [FromServices] IRequestHandler<GetTodoStatsQuery, Result<TodoStatsDto>> handler,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var query = new GetTodoStatsQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new todo
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoDto>> CreateTodo(
        [FromServices] IRequestHandler<CreateTodoCommand, Result<TodoDto>> handler,
        [FromBody] CreateTodoDto createTodoDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var command = new CreateTodoCommand(userId, createTodoDto);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(
            nameof(GetTodo),
            new { id = result.Value.Id },
            result.Value);
    }

    /// <summary>
    /// Update an existing todo
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TodoDto>> UpdateTodo(
        [FromServices] IRequestHandler<UpdateTodoCommand, Result<TodoDto>> handler,
        int id,
        [FromBody] UpdateTodoDto updateTodoDto,
        CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var command = new UpdateTodoCommand(id, userId, updateTodoDto);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Todo not found")
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Complete a todo
    /// </summary>
    [HttpPost("{id:int}/complete")]
    public async Task<ActionResult<TodoDto>> CompleteTodo(
        [FromServices] IRequestHandler<CompleteTodoCommand, Result<TodoDto>> handler,
        int id,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var command = new CompleteTodoCommand(id, userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Todo not found")
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Reopen a completed todo
    /// </summary>
    [HttpPost("{id:int}/reopen")]
    public async Task<ActionResult<TodoDto>> ReopenTodo(
        [FromServices] IRequestHandler<ReopenTodoCommand, Result<TodoDto>> handler,
        int id,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var command = new ReopenTodoCommand(id, userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Todo not found")
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Delete a todo
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteTodo(
        [FromServices] IRequestHandler<DeleteTodoCommand, Result<bool>> handler,
        int id,
        CancellationToken cancellationToken = default)
    {
        // TODO: Get userId from authentication context
        var userId = UserId.Create("1");

        var command = new DeleteTodoCommand(id, userId);
        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error == "Todo not found")
            {
                return NotFound();
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}
