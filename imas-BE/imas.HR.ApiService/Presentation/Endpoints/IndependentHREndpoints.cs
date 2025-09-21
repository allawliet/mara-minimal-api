using imas.HR.ApiService.Models;
using imas.HR.ApiService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace imas.HR.ApiService.Presentation.Endpoints;

/// <summary>
/// Independent HR Endpoints - no shared dependencies
/// </summary>
public static class IndependentHREndpoints
{
    public static void MapIndependentHREndpoints(this IEndpointRouteBuilder app)
    {
        var hrGroup = app.MapGroup("/api/hr").WithTags("HR Management").RequireAuthorization();

        // Employee endpoints
        MapEmployeeEndpoints(hrGroup);
        
        // Department endpoints
        MapDepartmentEndpoints(hrGroup);
        
        // Leave endpoints
        MapLeaveEndpoints(hrGroup);
        
        // Payroll endpoints
        MapPayrollEndpoints(hrGroup);
    }

    private static void MapEmployeeEndpoints(RouteGroupBuilder group)
    {
        var employees = group.MapGroup("/employees").WithTags("Employees");

        employees.MapGet("/", async (IHRService hrService) =>
        {
            var result = await hrService.GetAllEmployeesAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllEmployees")
        .WithSummary("Get all employees");

        employees.MapGet("/{id:int}", async (int id, IHRService hrService) =>
        {
            var result = await hrService.GetEmployeeByIdAsync(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetEmployeeById")
        .WithSummary("Get employee by ID");

        employees.MapPost("/", async ([FromBody] Employee employee, IHRService hrService) =>
        {
            var result = await hrService.CreateEmployeeAsync(employee);
            return Results.Created($"/api/hr/employees/{result.Id}", result);
        })
        .WithName("CreateEmployee")
        .WithSummary("Create new employee");

        employees.MapPut("/{id:int}", async (int id, [FromBody] Employee employee, IHRService hrService) =>
        {
            var result = await hrService.UpdateEmployeeAsync(id, employee);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("UpdateEmployee")
        .WithSummary("Update employee");

        employees.MapDelete("/{id:int}", async (int id, IHRService hrService) =>
        {
            var success = await hrService.DeleteEmployeeAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteEmployee")
        .WithSummary("Delete employee");
    }

    private static void MapDepartmentEndpoints(RouteGroupBuilder group)
    {
        var departments = group.MapGroup("/departments").WithTags("Departments");

        departments.MapGet("/", async (IHRService hrService) =>
        {
            var result = await hrService.GetAllDepartmentsAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllDepartments")
        .WithSummary("Get all departments");

        departments.MapGet("/{id:int}", async (int id, IHRService hrService) =>
        {
            var result = await hrService.GetDepartmentByIdAsync(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetDepartmentById")
        .WithSummary("Get department by ID");

        departments.MapPost("/", async ([FromBody] Department department, IHRService hrService) =>
        {
            var result = await hrService.CreateDepartmentAsync(department);
            return Results.Created($"/api/hr/departments/{result.Id}", result);
        })
        .WithName("CreateDepartment")
        .WithSummary("Create new department");

        departments.MapPut("/{id:int}", async (int id, [FromBody] Department department, IHRService hrService) =>
        {
            var result = await hrService.UpdateDepartmentAsync(id, department);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("UpdateDepartment")
        .WithSummary("Update department");

        departments.MapDelete("/{id:int}", async (int id, IHRService hrService) =>
        {
            var success = await hrService.DeleteDepartmentAsync(id);
            return success ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteDepartment")
        .WithSummary("Delete department");
    }

    private static void MapLeaveEndpoints(RouteGroupBuilder group)
    {
        var leave = group.MapGroup("/leave").WithTags("Leave Management");

        leave.MapGet("/", async (IHRService hrService) =>
        {
            var result = await hrService.GetLeaveRequestsAsync();
            return Results.Ok(result);
        })
        .WithName("GetLeaveRequests")
        .WithSummary("Get all leave requests");

        leave.MapGet("/{id:int}", async (int id, IHRService hrService) =>
        {
            var result = await hrService.GetLeaveRequestByIdAsync(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetLeaveRequestById")
        .WithSummary("Get leave request by ID");

        leave.MapPost("/", async ([FromBody] LeaveRequest leaveRequest, IHRService hrService) =>
        {
            var result = await hrService.CreateLeaveRequestAsync(leaveRequest);
            return Results.Created($"/api/hr/leave/{result.Id}", result);
        })
        .WithName("CreateLeaveRequest")
        .WithSummary("Create new leave request");

        leave.MapPut("/{id:int}", async (int id, [FromBody] LeaveRequest leaveRequest, IHRService hrService) =>
        {
            var result = await hrService.UpdateLeaveRequestAsync(id, leaveRequest);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("UpdateLeaveRequest")
        .WithSummary("Update leave request");
    }

    private static void MapPayrollEndpoints(RouteGroupBuilder group)
    {
        var payroll = group.MapGroup("/payroll").WithTags("Payroll Management");

        payroll.MapGet("/", async (IHRService hrService) =>
        {
            var result = await hrService.GetPayrollRecordsAsync();
            return Results.Ok(result);
        })
        .WithName("GetPayrollRecords")
        .WithSummary("Get all payroll records");

        payroll.MapGet("/{id:int}", async (int id, IHRService hrService) =>
        {
            var result = await hrService.GetPayrollRecordByIdAsync(id);
            return result != null ? Results.Ok(result) : Results.NotFound();
        })
        .WithName("GetPayrollRecordById")
        .WithSummary("Get payroll record by ID");

        payroll.MapPost("/", async ([FromBody] PayrollRecord payrollRecord, IHRService hrService) =>
        {
            var result = await hrService.CreatePayrollRecordAsync(payrollRecord);
            return Results.Created($"/api/hr/payroll/{result.Id}", result);
        })
        .WithName("CreatePayrollRecord")
        .WithSummary("Create new payroll record");
    }
}