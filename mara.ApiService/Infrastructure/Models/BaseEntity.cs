namespace mara.ApiService.Infrastructure.Models;

/// <summary>
/// Base interface for all entities with an ID
/// </summary>
public interface IEntity<TId>
{
    TId Id { get; set; }
}

/// <summary>
/// Base interface for entities with creation tracking
/// </summary>
public interface ICreatedEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
}

/// <summary>
/// Base interface for entities with modification tracking
/// </summary>
public interface IModifiedEntity
{
    DateTime? ModifiedAt { get; set; }
    string? ModifiedBy { get; set; }
}

/// <summary>
/// Base interface for entities with soft delete capability
/// </summary>
public interface ISoftDeleteEntity
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}

/// <summary>
/// Complete auditable entity interface
/// </summary>
public interface IAuditableEntity : ICreatedEntity, IModifiedEntity, ISoftDeleteEntity
{
}

/// <summary>
/// Base entity class with ID
/// </summary>
public abstract class BaseEntity<TId> : IEntity<TId>
{
    public TId Id { get; set; } = default!;
}

/// <summary>
/// Auditable entity with full tracking capabilities
/// </summary>
public abstract class AuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
