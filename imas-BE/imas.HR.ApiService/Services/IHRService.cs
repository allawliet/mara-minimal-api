using imas.HR.ApiService.Models;

namespace imas.HR.ApiService.Services;

/// <summary>
/// Local HR service interface - independent of shared contracts
/// </summary>
public interface IHRService
{
    // Employee Management
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<Employee> CreateEmployeeAsync(Employee employee);
    Task<Employee?> UpdateEmployeeAsync(int id, Employee employee);
    Task<bool> DeleteEmployeeAsync(int id);
    
    // Department Management
    Task<IEnumerable<Department>> GetAllDepartmentsAsync();
    Task<Department?> GetDepartmentByIdAsync(int id);
    Task<Department> CreateDepartmentAsync(Department department);
    Task<Department?> UpdateDepartmentAsync(int id, Department department);
    Task<bool> DeleteDepartmentAsync(int id);
    
    // Leave Management
    Task<IEnumerable<LeaveRequest>> GetLeaveRequestsAsync();
    Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
    Task<LeaveRequest> CreateLeaveRequestAsync(LeaveRequest leaveRequest);
    Task<LeaveRequest?> UpdateLeaveRequestAsync(int id, LeaveRequest leaveRequest);
    
    // Payroll Management
    Task<IEnumerable<PayrollRecord>> GetPayrollRecordsAsync();
    Task<PayrollRecord?> GetPayrollRecordByIdAsync(int id);
    Task<PayrollRecord> CreatePayrollRecordAsync(PayrollRecord payrollRecord);
}