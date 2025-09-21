using imas.ApiService.Domain.Common;

namespace imas.ApiService.Domain.Todos.ValueObjects;

/// <summary>
/// Todo title value object
/// </summary>
public sealed class TodoTitle : ValueObject
{
    public string Value { get; }

    private TodoTitle(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Todo title cannot be null or empty", nameof(value));
        
        if (value.Length > 200)
            throw new ArgumentException("Todo title cannot exceed 200 characters", nameof(value));

        Value = value.Trim();
    }

    public static TodoTitle Create(string value) => new(value);

    public static implicit operator string(TodoTitle title) => title.Value;
    public static implicit operator TodoTitle(string value) => Create(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Todo description value object
/// </summary>
public sealed class TodoDescription : ValueObject
{
    public string? Value { get; }

    private TodoDescription(string? value)
    {
        if (value?.Length > 1000)
            throw new ArgumentException("Todo description cannot exceed 1000 characters", nameof(value));

        Value = value?.Trim();
    }

    public static TodoDescription Create(string? value) => new(value);

    public static implicit operator string?(TodoDescription? description) => description?.Value;
    public static implicit operator TodoDescription(string? value) => Create(value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value ?? string.Empty;
    }

    public override string ToString() => Value ?? string.Empty;
}
