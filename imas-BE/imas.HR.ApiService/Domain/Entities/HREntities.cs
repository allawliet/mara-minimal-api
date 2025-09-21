using imas.HR.ApiService.Domain.Common;

namespace imas.HR.ApiService.Domain.Entities;

public class Employee : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public DateTime? HireDate { get; set; }
    public decimal? Salary { get; set; }
    public string Status { get; set; } = "Active";
    public string? Manager { get; set; }

    // Navigation properties
    public Department? Department { get; set; }
    public Position? Position { get; set; }

    // Domain methods
    public string GetFullName() => $"{FirstName} {LastName}";
    
    public bool IsActive() => Status == "Active";
    
    public void UpdateContactInfo(string email, string? phone, string? address)
    {
        Email = email;
        Phone = phone;
        Address = address;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void PromoteToPosition(int newPositionId, decimal newSalary)
    {
        PositionId = newPositionId;
        Salary = newSalary;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
    public string? Budget { get; set; }

    // Navigation properties
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    // Domain methods
    public int GetEmployeeCount() => Employees.Count;
}

public class Position : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public string? Requirements { get; set; }

    // Navigation properties
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    // Domain methods
    public bool IsSalaryInRange(decimal salary) =>
        (MinSalary == null || salary >= MinSalary) &&
        (MaxSalary == null || salary <= MaxSalary);
}

public class Attendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? CheckInTime { get; set; }
    public TimeSpan? CheckOutTime { get; set; }
    public string Status { get; set; } = "Present"; // Present, Absent, Late, Holiday
    public string? Notes { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;

    // Domain methods
    public TimeSpan? GetWorkingHours()
    {
        if (CheckInTime.HasValue && CheckOutTime.HasValue)
            return CheckOutTime.Value - CheckInTime.Value;
        return null;
    }

    public bool IsLate(TimeSpan standardStartTime) =>
        CheckInTime.HasValue && CheckInTime.Value > standardStartTime;
}