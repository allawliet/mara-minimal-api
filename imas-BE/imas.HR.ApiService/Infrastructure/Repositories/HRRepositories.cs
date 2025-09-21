using imas.HR.ApiService.Application.Employees;
using imas.HR.ApiService.Domain.Entities;
using imas.HR.ApiService.Data;
using Microsoft.EntityFrameworkCore;

namespace imas.HR.ApiService.Infrastructure.Repositories;

public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(HRDbContext context) : base(context) { }

    public override async Task<Employee?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public override async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .ToListAsync();
    }

    public async Task<Employee?> GetByEmployeeIdAsync(string employeeId)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

    public async Task<IEnumerable<Employee>> GetByDepartmentAsync(int departmentId)
    {
        return await _dbSet
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Where(e => e.DepartmentId == departmentId)
            .ToListAsync();
    }
}

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(HRDbContext context) : base(context) { }

    public async Task<Department?> GetWithEmployeesAsync(int id)
    {
        return await _dbSet
            .Include(d => d.Employees)
            .FirstOrDefaultAsync(d => d.Id == id);
    }
}

public class PositionRepository : BaseRepository<Position>, IPositionRepository
{
    public PositionRepository(HRDbContext context) : base(context) { }

    public override async Task<IEnumerable<Position>> GetAllAsync()
    {
        return await _dbSet
            .Include(p => p.Employees)
            .ToListAsync();
    }
}

public class AttendanceRepository : BaseRepository<Attendance>, IAttendanceRepository
{
    public AttendanceRepository(HRDbContext context) : base(context) { }

    public override async Task<IEnumerable<Attendance>> GetAllAsync()
    {
        return await _dbSet
            .Include(a => a.Employee)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Attendance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(a => a.Employee)
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .OrderBy(a => a.Date)
            .ToListAsync();
    }
}