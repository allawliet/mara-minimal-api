using imas.HR.ApiService.Application.Common;
using imas.HR.ApiService.Application.Employees;
using imas.HR.ApiService.Domain.Entities;

namespace imas.HR.ApiService.Application.Employees;

public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<ApiResponse<IEnumerable<DepartmentDto>>> GetAllDepartmentsAsync()
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync();
            var departmentDtos = departments.Select(MapToDto);
            return ApiResponse<IEnumerable<DepartmentDto>>.SuccessResponse(departmentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<DepartmentDto>>.ErrorResponse($"Error retrieving departments: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DepartmentDto?>> GetDepartmentByIdAsync(int id)
    {
        try
        {
            var department = await _departmentRepository.GetWithEmployeesAsync(id);
            if (department == null)
                return ApiResponse<DepartmentDto?>.ErrorResponse("Department not found");

            var departmentDto = MapToDto(department);
            return ApiResponse<DepartmentDto?>.SuccessResponse(departmentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<DepartmentDto?>.ErrorResponse($"Error retrieving department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<DepartmentDto>> CreateDepartmentAsync(CreateDepartmentRequest request)
    {
        try
        {
            var department = new Department
            {
                Name = request.Name,
                Description = request.Description,
                ManagerId = request.ManagerId,
                Budget = request.Budget
            };

            var createdDepartment = await _departmentRepository.AddAsync(department);
            var departmentDto = MapToDto(createdDepartment);
            return ApiResponse<DepartmentDto>.SuccessResponse(departmentDto, "Department created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<DepartmentDto>.ErrorResponse($"Error creating department: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteDepartmentAsync(int id)
    {
        try
        {
            var result = await _departmentRepository.DeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("Department not found or could not be deleted");

            return ApiResponse<bool>.SuccessResponse(true, "Department deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Error deleting department: {ex.Message}");
        }
    }

    private static DepartmentDto MapToDto(Department department) => new(
        department.Id,
        department.Name,
        department.Description,
        department.ManagerId,
        department.Budget,
        department.GetEmployeeCount(),
        department.CreatedAt
    );
}

public class PositionService : IPositionService
{
    private readonly IPositionRepository _positionRepository;

    public PositionService(IPositionRepository positionRepository)
    {
        _positionRepository = positionRepository;
    }

    public async Task<ApiResponse<IEnumerable<PositionDto>>> GetAllPositionsAsync()
    {
        try
        {
            var positions = await _positionRepository.GetAllAsync();
            var positionDtos = positions.Select(MapToDto);
            return ApiResponse<IEnumerable<PositionDto>>.SuccessResponse(positionDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<PositionDto>>.ErrorResponse($"Error retrieving positions: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PositionDto?>> GetPositionByIdAsync(int id)
    {
        try
        {
            var position = await _positionRepository.GetByIdAsync(id);
            if (position == null)
                return ApiResponse<PositionDto?>.ErrorResponse("Position not found");

            var positionDto = MapToDto(position);
            return ApiResponse<PositionDto?>.SuccessResponse(positionDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PositionDto?>.ErrorResponse($"Error retrieving position: {ex.Message}");
        }
    }

    public async Task<ApiResponse<PositionDto>> CreatePositionAsync(CreatePositionRequest request)
    {
        try
        {
            var position = new Position
            {
                Title = request.Title,
                Description = request.Description,
                MinSalary = request.MinSalary,
                MaxSalary = request.MaxSalary,
                Requirements = request.Requirements
            };

            var createdPosition = await _positionRepository.AddAsync(position);
            var positionDto = MapToDto(createdPosition);
            return ApiResponse<PositionDto>.SuccessResponse(positionDto, "Position created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<PositionDto>.ErrorResponse($"Error creating position: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeletePositionAsync(int id)
    {
        try
        {
            var result = await _positionRepository.DeleteAsync(id);
            if (!result)
                return ApiResponse<bool>.ErrorResponse("Position not found or could not be deleted");

            return ApiResponse<bool>.SuccessResponse(true, "Position deleted successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.ErrorResponse($"Error deleting position: {ex.Message}");
        }
    }

    private static PositionDto MapToDto(Position position) => new(
        position.Id,
        position.Title,
        position.Description,
        position.MinSalary,
        position.MaxSalary,
        position.Requirements,
        position.CreatedAt
    );
}

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendanceRepository;

    public AttendanceService(IAttendanceRepository attendanceRepository)
    {
        _attendanceRepository = attendanceRepository;
    }

    public async Task<ApiResponse<IEnumerable<AttendanceDto>>> GetAllAttendanceAsync()
    {
        try
        {
            var attendances = await _attendanceRepository.GetAllAsync();
            var attendanceDtos = attendances.Select(MapToDto);
            return ApiResponse<IEnumerable<AttendanceDto>>.SuccessResponse(attendanceDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AttendanceDto>>.ErrorResponse($"Error retrieving attendance: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AttendanceDto?>> GetAttendanceByIdAsync(int id)
    {
        try
        {
            var attendance = await _attendanceRepository.GetByIdAsync(id);
            if (attendance == null)
                return ApiResponse<AttendanceDto?>.ErrorResponse("Attendance record not found");

            var attendanceDto = MapToDto(attendance);
            return ApiResponse<AttendanceDto?>.SuccessResponse(attendanceDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<AttendanceDto?>.ErrorResponse($"Error retrieving attendance: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<AttendanceDto>>> GetEmployeeAttendanceAsync(int employeeId)
    {
        try
        {
            var attendances = await _attendanceRepository.GetByEmployeeIdAsync(employeeId);
            var attendanceDtos = attendances.Select(MapToDto);
            return ApiResponse<IEnumerable<AttendanceDto>>.SuccessResponse(attendanceDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<AttendanceDto>>.ErrorResponse($"Error retrieving employee attendance: {ex.Message}");
        }
    }

    public async Task<ApiResponse<AttendanceDto>> CreateAttendanceAsync(CreateAttendanceRequest request)
    {
        try
        {
            var attendance = new Attendance
            {
                EmployeeId = request.EmployeeId,
                Date = request.Date,
                CheckInTime = request.CheckInTime,
                CheckOutTime = request.CheckOutTime,
                Status = request.Status,
                Notes = request.Notes
            };

            var createdAttendance = await _attendanceRepository.AddAsync(attendance);
            var attendanceDto = MapToDto(createdAttendance);
            return ApiResponse<AttendanceDto>.SuccessResponse(attendanceDto, "Attendance record created successfully");
        }
        catch (Exception ex)
        {
            return ApiResponse<AttendanceDto>.ErrorResponse($"Error creating attendance record: {ex.Message}");
        }
    }

    private static AttendanceDto MapToDto(Attendance attendance) => new(
        attendance.Id,
        attendance.EmployeeId,
        attendance.Employee?.GetFullName() ?? "Unknown",
        attendance.Date,
        attendance.CheckInTime,
        attendance.CheckOutTime,
        attendance.Status,
        attendance.Notes,
        attendance.GetWorkingHours()
    );
}