using imas.HR.ApiService.Application.Common;
using imas.HR.ApiService.Application.Employees;
using imas.HR.ApiService.Domain.Entities;

namespace imas.HR.ApiService.Application.Employees;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IPositionRepository _positionRepository;

    public EmployeeService(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
    }

    public async Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync();
            var employeeDtos = employees.Select(MapToDto);
            return ApiResponse<IEnumerable<EmployeeDto>>.SuccessResponse(employeeDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<EmployeeDto>>.ErrorResponse($"Error retrieving employees: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(int id)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<EmployeeDto?>.ErrorResponse("Employee not found");

            var employeeDto = MapToDto(employee);
            return ApiResponse<EmployeeDto?>.SuccessResponse(employeeDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto?>.ErrorResponse($"Error retrieving employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeRequest request)
    {
        try
        {
            var employee = new Employee
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Address = request.Address,
                EmployeeId = request.EmployeeId,
                DepartmentId = request.DepartmentId,
                PositionId = request.PositionId,
                HireDate = request.HireDate,
                Salary = request.Salary,
                Manager = request.Manager,
                Status = "Active"
            };

            var createdEmployee = await _employeeRepository.AddAsync(employee);
            var employeeDto = MapToDto(createdEmployee);
            return ApiResponse<EmployeeDto>.SuccessResponse(employeeDto, "Employee created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto>.ErrorResponse($"Error creating employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EmployeeDto?>> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null)
                return ApiResponse<EmployeeDto?>.ErrorResponse("Employee not found");

            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            employee.Address = request.Address;
            employee.DepartmentId = request.DepartmentId;
            employee.PositionId = request.PositionId;
            employee.Salary = request.Salary;
            employee.Status = request.Status;
            employee.Manager = request.Manager;
            employee.UpdatedAt = DateTime.UtcNow;

            var updatedEmployee = await _employeeRepository.UpdateAsync(employee);
            var employeeDto = MapToDto(updatedEmployee);
            return ApiResponse<EmployeeDto?>.SuccessResponse(employeeDto, "Employee updated successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<EmployeeDto?>.ErrorResponse($"Error updating employee: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteEmployeeAsync(int id)
    {
        try
        {
            var result = await _employeeRepository.DeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("Employee not found or could not be deleted");

            return ApiResponse<bool>.SuccessResponse(true, "Employee deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Error deleting employee: {ex.Message}");
        }
    }

    private static EmployeeDto MapToDto(Employee employee) => new(
        employee.Id,
        employee.FirstName,
        employee.LastName,
        employee.Email,
        employee.Phone,
        employee.Address,
        employee.EmployeeId,
        employee.DepartmentId,
        employee.Department?.Name,
        employee.PositionId,
        employee.Position?.Title,
        employee.HireDate,
        employee.Salary,
        employee.Status,
        employee.Manager,
        employee.CreatedAt,
        employee.UpdatedAt
    );
}