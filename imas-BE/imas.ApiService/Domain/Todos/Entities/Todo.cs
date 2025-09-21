using imas.ApiService.Domain.Common;
using imas.ApiService.Domain.Todos.ValueObjects;
using imas.ApiService.Domain.Todos.Events;

namespace imas.ApiService.Domain.Todos.Entities;

/// <summary>
/// Todo aggregate root
/// </summary>
public sealed class Todo : AggregateRoot<int>, IAuditableEntity, ISoftDeletable
{
    private TodoTitle _title = null!;
    private TodoDescription? _description;
    private bool _isCompleted;
    private DateTime? _completedAt;

    // Required for EF Core
    private Todo() { }

    private Todo(TodoTitle title, TodoDescription? description, UserId userId)
        : base()
    {
        _title = title;
        _description = description;
        _isCompleted = false;
        CreatedBy = userId;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new TodoCreated(Id, userId, title, description));
    }

    public static Todo Create(TodoTitle title, TodoDescription? description, UserId userId)
    {
        return new Todo(title, description, userId);
    }

    // Properties
    public TodoTitle Title => _title;
    public TodoDescription? Description => _description;
    public bool IsCompleted => _isCompleted;
    public DateTime? CompletedAt => _completedAt;
    public UserId UserId => CreatedBy;

    // Auditable properties
    public DateTime CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = null!;
    public DateTime? ModifiedAt { get; private set; }
    public string? ModifiedBy { get; private set; }

    // Soft delete properties
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }
    public string? DeletedBy { get; private set; }

    // Business methods
    public void UpdateTitle(TodoTitle newTitle, UserId userId)
    {
        if (_title != newTitle)
        {
            _title = newTitle;
            UpdateModifiedFields(userId);
            AddDomainEvent(new TodoUpdated(Id, userId, _title, _description));
        }
    }

    public void UpdateDescription(TodoDescription? newDescription, UserId userId)
    {
        if (!object.Equals(_description, newDescription))
        {
            _description = newDescription;
            UpdateModifiedFields(userId);
            AddDomainEvent(new TodoUpdated(Id, userId, _title, _description));
        }
    }

    public void Update(TodoTitle title, TodoDescription? description, UserId userId)
    {
        var hasChanges = _title != title || !object.Equals(_description, description);
        
        if (hasChanges)
        {
            _title = title;
            _description = description;
            UpdateModifiedFields(userId);
            AddDomainEvent(new TodoUpdated(Id, userId, _title, _description));
        }
    }

    public void Complete(UserId userId)
    {
        if (!_isCompleted)
        {
            _isCompleted = true;
            _completedAt = DateTime.UtcNow;
            UpdateModifiedFields(userId);
            AddDomainEvent(new TodoCompleted(Id, userId, _title, _completedAt.Value));
        }
    }

    public void Reopen(UserId userId)
    {
        if (_isCompleted)
        {
            _isCompleted = false;
            _completedAt = null;
            UpdateModifiedFields(userId);
            AddDomainEvent(new TodoReopened(Id, userId, _title));
        }
    }

    public void Delete(UserId userId)
    {
        if (!IsDeleted)
        {
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            DeletedBy = userId;
            AddDomainEvent(new TodoDeleted(Id, userId, _title));
        }
    }

    private void UpdateModifiedFields(UserId userId)
    {
        ModifiedAt = DateTime.UtcNow;
        ModifiedBy = userId;
    }

    // For EF Core navigation
    public void SetId(int id)
    {
        Id = id;
    }
}
