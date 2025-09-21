using Microsoft.EntityFrameworkCore;
using imas.HR.ApiService.Data;
using imas.HR.ApiService.Models;

namespace imas.HR.ApiService.Services;

/// <summary>
/// Independent HR Service implementation - no shared dependencies
/// </summary>
public class HRService : IHRService
{
    private readonly HRDbContext _context;
    private readonly ILogger<HRService> _logger;

    public HRService(HRDbContext context, ILogger<HRService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // Employee Management
    public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
    {
        return await _context.Employees.ToListAsync();
    }

    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees.FindAsync(id);
    }

    public async Task<Employee> CreateEmployeeAsync(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee?> UpdateEmployeeAsync(int id, Employee employee)
    {
        var existingEmployee = await _context.Employees.FindAsync(id);
        if (existingEmployee == null) return null;

        existingEmployee.FirstName = employee.FirstName;
        existingEmployee.LastName = employee.LastName;
        existingEmployee.Email = employee.Email;
        existingEmployee.PhoneNumber = employee.PhoneNumber;
        existingEmployee.JobTitle = employee.JobTitle;
        existingEmployee.Salary = employee.Salary;
        existingEmployee.DepartmentId = employee.DepartmentId;
        existingEmployee.Status = employee.Status;

        await _context.SaveChangesAsync();
        return existingEmployee;
    }

    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return false;

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    // Department Management
    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments.ToListAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments.FindAsync(id);
    }

    public async Task<Department> CreateDepartmentAsync(Department department)
    {
        _context.Departments.Add(department);
        await _context.SaveChangesAsync();
        return department;
    }

    public async Task<Department?> UpdateDepartmentAsync(int id, Department department)
    {
        var existingDepartment = await _context.Departments.FindAsync(id);
        if (existingDepartment == null) return null;

        existingDepartment.Name = department.Name;
        existingDepartment.Description = department.Description;
        existingDepartment.ManagerId = department.ManagerId;

        await _context.SaveChangesAsync();
        return existingDepartment;
    }

    public async Task<bool> DeleteDepartmentAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return false;

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }

    // Leave Management
    public async Task<IEnumerable<LeaveRequest>> GetLeaveRequestsAsync()
    {
        return await _context.LeaveRequests.ToListAsync();
    }

    public async Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id)
    {
        return await _context.LeaveRequests.FindAsync(id);
    }

    public async Task<LeaveRequest> CreateLeaveRequestAsync(LeaveRequest leaveRequest)
    {
        _context.LeaveRequests.Add(leaveRequest);
        await _context.SaveChangesAsync();
        return leaveRequest;
    }

    public async Task<LeaveRequest?> UpdateLeaveRequestAsync(int id, LeaveRequest leaveRequest)
    {
        var existingRequest = await _context.LeaveRequests.FindAsync(id);
        if (existingRequest == null) return null;

        existingRequest.StartDate = leaveRequest.StartDate;
        existingRequest.EndDate = leaveRequest.EndDate;
        existingRequest.Type = leaveRequest.Type;
        existingRequest.Status = leaveRequest.Status;
        existingRequest.Reason = leaveRequest.Reason;

        await _context.SaveChangesAsync();
        return existingRequest;
    }

    // Payroll Management
    public async Task<IEnumerable<PayrollRecord>> GetPayrollRecordsAsync()
    {
        return await _context.PayrollRecords.ToListAsync();
    }

    public async Task<PayrollRecord?> GetPayrollRecordByIdAsync(int id)
    {
        return await _context.PayrollRecords.FindAsync(id);
    }

    public async Task<PayrollRecord> CreatePayrollRecordAsync(PayrollRecord payrollRecord)
    {
        _context.PayrollRecords.Add(payrollRecord);
        await _context.SaveChangesAsync();
        return payrollRecord;
    }
}