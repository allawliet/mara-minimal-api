using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.ValueObjects;

namespace imas.ApiService.Domain.Todos.Events;

/// <summary>
/// Domain events for Todo aggregate
/// </summary>
public record TodoCreated(
    int TodoId,
    UserId UserId,
    TodoTitle Title,
    TodoDescription? Description) : DomainEvent;

public record TodoCompleted(
    int TodoId,
    UserId UserId,
    TodoTitle Title,
    DateTime CompletedAt) : DomainEvent;

public record TodoUpdated(
    int TodoId,
    UserId UserId,
    TodoTitle Title,
    TodoDescription? Description) : DomainEvent;

public record TodoDeleted(
    int TodoId,
    UserId UserId,
    TodoTitle Title) : DomainEvent;

public record TodoReopened(
    int TodoId,
    UserId UserId,
    TodoTitle Title) : DomainEvent;
