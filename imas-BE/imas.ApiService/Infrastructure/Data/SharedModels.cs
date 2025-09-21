using imas.ApiService.Modules.Authentication;
using imas.ApiService.Modules.Todos;
using imas.ApiService.Modules.Weather;

namespace imas.ApiService.Infrastructure.Data;

// HR Service Models
public class HREmployee
{
    public int Id { get; set; }
    public string EmployeeNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public int DepartmentId { get; set; }
    public EmployeeStatus Status { get; set; }
    public string? JobTitle { get; set; }
    public int? ManagerId { get; set; }
}

public class HRDepartment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveStatus Status { get; set; }
    public DateTime RequestDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public int? ApprovedBy { get; set; }
}

public class PayrollRecord
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime PayPeriodStart { get; set; }
    public DateTime PayPeriodEnd { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
    public DateTime PaymentDate { get; set; }
}

// Assets Service Models
public class Asset
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AssetTag { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public decimal CurrentValue { get; set; }
    public AssetStatus Status { get; set; }
    public string? Location { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public DateTime? WarrantyExpiry { get; set; }
}

public class AssetCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal? DepreciationRate { get; set; }
    public int? LifespanYears { get; set; }
}

public class MaintenanceRecord
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public DateTime MaintenanceDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Cost { get; set; }
    public string? TechnicianName { get; set; }
    public MaintenanceType Type { get; set; }
    public DateTime? NextMaintenanceDate { get; set; }
}

public class AssetAssignment
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public string AssignedTo { get; set; } = string.Empty;
    public DateTime AssignedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }
    public string? Notes { get; set; }
    public AssignmentStatus Status { get; set; }
}

// Finance Service Models
public class Invoice
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string? Description { get; set; }
    public decimal? PaidAmount { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Reference { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Notes { get; set; }
}

public class Budget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BudgetStatus Status { get; set; }
}

public class Expense
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string Category { get; set; } = string.Empty;
    public int? BudgetId { get; set; }
    public string? Receipt { get; set; }
    public ExpenseStatus Status { get; set; }
    public string? ApprovedBy { get; set; }
}

// General Service Models
public class GeneralDepartment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? HeadOfDepartment { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Permissions { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsActive { get; set; }
}

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public bool IsActive { get; set; }
}

// Enums
public enum EmployeeStatus
{
    Active = 1,
    Inactive = 2,
    Terminated = 3,
    OnLeave = 4
}

public enum LeaveStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Cancelled = 4
}

public enum AssetStatus
{
    Available = 1,
    InUse = 2,
    UnderMaintenance = 3,
    Disposed = 4,
    Lost = 5
}

public enum MaintenanceType
{
    Preventive = 1,
    Corrective = 2,
    Emergency = 3
}

public enum AssignmentStatus
{
    Active = 1,
    Returned = 2,
    Lost = 3,
    Damaged = 4
}

public enum InvoiceStatus
{
    Draft = 1,
    Sent = 2,
    Paid = 3,
    Overdue = 4,
    Cancelled = 5
}

public enum PaymentStatus
{
    Pending = 1,
    Completed = 2,
    Failed = 3,
    Cancelled = 4
}

public enum BudgetStatus
{
    Active = 1,
    Completed = 2,
    Cancelled = 3,
    OnHold = 4
}

public enum ExpenseStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3,
    Paid = 4
}