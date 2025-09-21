namespace imas.HR.ApiService.Models;

// Independent enums - no longer shared
public enum EmployeeStatus
{
    Active,
    Inactive,
    Terminated,
    OnLeave
}

public enum DepartmentStatus
{
    Active,
    Inactive
}

public enum LeaveType
{
    Vacation,
    Sick,
    Personal,
    Maternity,
    Paternity
}

public enum LeaveStatus
{
    Pending,
    Approved,
    Rejected,
    Cancelled
}

public enum PayrollStatus
{
    Draft,
    Processed,
    Paid,
    Cancelled
}