using imas.HR.ApiService.Application.Common;
using imas.HR.ApiService.Domain.Entities;

namespace imas.HR.ApiService.Application.Employees;

public interface IEmployeeService
{
    Task<ApiResponse<IEnumerable<EmployeeDto>>> GetAllEmployeesAsync();
    Task<ApiResponse<EmployeeDto?>> GetEmployeeByIdAsync(int id);
    Task<ApiResponse<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<ApiResponse<EmployeeDto?>> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
    Task<ApiResponse<bool>> DeleteEmployeeAsync(int id);
}

public interface IDepartmentService
{
    Task<ApiResponse<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync();
    Task<ApiResponse<DepartmentDto?>> GetDepartmentByIdAsync(int id);
    Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentRequest request);
    Task<ApiResponse<bool>> DeleteDepartmentAsync(int id);
}

public interface IPositionService
{
    Task<ApiResponse<IEnumerable<PositionDto>>> GetAllPositionsAsync();
    Task<ApiResponse<PositionDto?>> GetPositionByIdAsync(int id);
    Task<ApiResponse<PositionDto>> CreatePositionAsync(CreatePositionRequest request);
    Task<ApiResponse<bool>> DeletePositionAsync(int id);
}

public interface IAttendanceService
{
    Task<ApiResponse<IEnumerable<AttendanceDto>>> GetAllAttendanceAsync();
    Task<ApiResponse<AttendanceDto?>> GetAttendanceByIdAsync(int id);
    Task<ApiResponse<IEnumerable<AttendanceDto>>> GetEmployeeAttendanceAsync(int employeeId);
    Task<ApiResponse<AttendanceDto>> CreateAttendanceAsync(CreateAttendanceRequest request);
}

// Repository interfaces
public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByEmployeeIdAsync(string employeeId);
    Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId);
}

public interface IDepartmentRepository : IRepository<Department>
{
    Task<Department?> GetWithEmployeesAsync(int id);
}

public interface IPositionRepository : IRepository<Position> { }

public interface IAttendanceRepository : IRepository<Attendance>
{
    Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<Attendance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
}