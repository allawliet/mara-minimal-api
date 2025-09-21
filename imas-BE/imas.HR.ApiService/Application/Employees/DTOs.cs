namespace imas.HR.ApiService.Application.Employees;

// DTOs
public record EmployeeDto(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Address,
    string EmployeeId,
    int? DepartmentId,
    string? DepartmentName,
    int? PositionId,
    string? PositionTitle,
    DateTime? HireDate,
    decimal? Salary,
    string Status,
    string? Manager,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Address,
    string EmployeeId,
    int? DepartmentId,
    int? PositionId,
    DateTime? HireDate,
    decimal? Salary,
    string? Manager
);

public record UpdateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Address,
    int? DepartmentId,
    int? PositionId,
    decimal? Salary,
    string Status,
    string? Manager
);

public record DepartmentDto(
    int Id,
    string Name,
    string? Description,
    int? ManagerId,
    string? Budget,
    int EmployeeCount,
    DateTime CreatedAt
);

public record CreateDepartmentRequest(
    string Name,
    string? Description,
    int? ManagerId,
    string? Budget
);

public record PositionDto(
    int Id,
    string Title,
    string? Description,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? Requirements,
    DateTime CreatedAt
);

public record CreatePositionRequest(
    string Title,
    string? Description,
    decimal? MinSalary,
    decimal? MaxSalary,
    string? Requirements
);

public record AttendanceDto(
    int Id,
    int EmployeeId,
    string EmployeeName,
    DateTime Date,
    TimeSpan? CheckInTime,
    TimeSpan? CheckOutTime,
    string Status,
    string? Notes,
    TimeSpan? WorkingHours
);

public record CreateAttendanceRequest(
    int EmployeeId,
    DateTime Date,
    TimeSpan? CheckInTime,
    TimeSpan? CheckOutTime,
    string Status,
    string? Notes
);