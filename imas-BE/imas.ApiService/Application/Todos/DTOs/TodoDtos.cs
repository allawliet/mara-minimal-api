namespace imas.ApiService.Application.Todos.DTOs;

/// <summary>
/// Todo DTO for API responses
/// </summary>
public record TodoDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

/// <summary>
/// Todo summary DTO for list views
/// </summary>
public record TodoSummaryDto
{
    public int Id { get; init; }
    public string Title { get; init; } = null!;
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Create todo request DTO
/// </summary>
public record CreateTodoDto
{
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
}

/// <summary>
/// Update todo request DTO
/// </summary>
public record UpdateTodoDto
{
    public string Title { get; init; } = null!;
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
}

/// <summary>
/// Todo statistics DTO
/// </summary>
public record TodoStatsDto
{
    public int TotalCount { get; init; }
    public int CompletedCount { get; init; }
    public int PendingCount { get; init; }
    public double CompletionRate { get; init; }
}
