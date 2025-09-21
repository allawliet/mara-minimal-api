namespace imas.ApiService.Domain.Common;

/// <summary>
/// Interface for auditable entities
/// </summary>
public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
    string CreatedBy { get; }
    DateTime? ModifiedAt { get; }
    string? ModifiedBy { get; }
}

/// <summary>
/// Interface for soft deletable entities
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    string? DeletedBy { get; }
}

/// <summary>
/// User identifier value object
/// </summary>
public sealed class UserId : ValueObject
{
    public string Value { get; }

    private UserId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("User ID cannot be null or empty", nameof(value));
        
        Value = value;
    }

    public static UserId Create(string value) => new(value);

    public static implicit operator string(UserId userId) => userId.Value;
    public static implicit operator UserId(string value) => Create(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}
