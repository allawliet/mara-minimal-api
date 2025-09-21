namespace imas.ApiService.Application.Common;

/// <summary>
/// Application result for operations that can succeed or fail
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    public static Result Success() => new(true);
    public static Result Failure(string error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true);
    public static Result<T> Failure<T>(string error) => new(default!, false, error);
}

/// <summary>
/// Application result with a value
/// </summary>
public class Result<T> : Result
{
    protected internal Result(T value, bool isSuccess, string? error = null)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public T Value { get; }

    public static implicit operator Result<T>(T value) => Success(value);
}

/// <summary>
/// Pagination result
/// </summary>
public class PagedResult<T>
{
    public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        HasNextPage = page < TotalPages;
        HasPreviousPage = page > 1;
    }

    public IEnumerable<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages { get; }
    public bool HasNextPage { get; }
    public bool HasPreviousPage { get; }
}
