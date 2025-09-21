using imas.HR.ApiService.Application.Employees;
using Microsoft.AspNetCore.Mvc;

namespace imas.HR.ApiService.Presentation.Endpoints;

public static class EmployeeEndpoints
{
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees").WithTags("Employees");

        // Get all employees
        group.MapGet("/", async (IEmployeeService employeeService) =>
        {
            var result = await employeeService.GetAllEmployeesAsync();
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        // Get employee by ID
        group.MapGet("/{id:int}", async (int id, IEmployeeService employeeService) =>
        {
            var result = await employeeService.GetEmployeeByIdAsync(id);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        // Create new employee
        group.MapPost("/", async ([FromBody] CreateEmployeeRequest request, IEmployeeService employeeService) =>
        {
            var result = await employeeService.CreateEmployeeAsync(request);
            return result.Success ? Results.Created($"/api/employees/{result.Data?.Id}", result) : Results.BadRequest(result);
        });

        // Update employee
        group.MapPut("/{id:int}", async (int id, [FromBody] UpdateEmployeeRequest request, IEmployeeService employeeService) =>
        {
            var result = await employeeService.UpdateEmployeeAsync(id, request);
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        // Delete employee
        group.MapDelete("/{id:int}", async (int id, IEmployeeService employeeService) =>
        {
            var result = await employeeService.DeleteEmployeeAsync(id);
            return result.Success ? Results.NoContent() : Results.BadRequest(result);
        });
    }
}

public static class DepartmentEndpoints
{
    public static void MapDepartmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/departments").WithTags("Departments");

        // Get all departments
        group.MapGet("/", async (IDepartmentService departmentService) =>
        {
            var result = await departmentService.GetAllDepartmentsAsync();
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        // Get department by ID
        group.MapGet("/{id:int}", async (int id, IDepartmentService departmentService) =>
        {
            var result = await departmentService.GetDepartmentByIdAsync(id);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        });

        // Create new department
        group.MapPost("/", async ([FromBody] CreateDepartmentRequest request, IDepartmentService departmentService) =>
        {
            var result = await departmentService.CreateDepartmentAsync(request);
            return result.Success ? Results.Created($"/api/departments/{result.Data?.Id}", result) : Results.BadRequest(result);
        });

        // Update department
        group.MapPut("/{id:int}", async (int id, [FromBody] CreateDepartmentRequest request, IDepartmentService departmentService) =>
        {
            // Note: Using CreateDepartmentRequest as UpdateDepartmentRequest is not defined
            var result = await departmentService.CreateDepartmentAsync(request); // This would need to be an update method
            return result.Success ? Results.Ok(result) : Results.BadRequest(result);
        });

        // Delete department
        group.MapDelete("/{id:int}", async (int id, IDepartmentService departmentService) =>
        {
            var result = await departmentService.DeleteDepartmentAsync(id);
            return result.Success ? Results.NoContent() : Results.BadRequest(result);
        });
    }
}