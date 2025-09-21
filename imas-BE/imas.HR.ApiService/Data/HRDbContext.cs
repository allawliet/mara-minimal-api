using Microsoft.EntityFrameworkCore;
using imas.HR.ApiService.Models;

namespace imas.HR.ApiService.Data;

public class HRDbContext : DbContext
{
    public HRDbContext(DbContextOptions<HRDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<PayrollRecord> PayrollRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure Department entity
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure LeaveRequest entity
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(1000);
            entity.HasOne<Employee>()
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PayrollRecord entity
        modelBuilder.Entity<PayrollRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Allowances).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Deductions).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NetSalary).HasColumnType("decimal(18,2)");
            entity.HasOne<Employee>()
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Seed data
        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        // Seed departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "Engineering", Description = "Software development and technical teams" },
            new Department { Id = 2, Name = "Human Resources", Description = "HR management and recruitment" },
            new Department { Id = 3, Name = "Finance", Description = "Financial planning and accounting" },
            new Department { Id = 4, Name = "Marketing", Description = "Marketing and sales teams" }
        );

        // Seed employees
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                EmployeeNumber = "EMP001",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@imas.com",
                PhoneNumber = "+1234567890",
                DateOfBirth = new DateTime(1985, 5, 15),
                HireDate = new DateTime(2020, 1, 15),
                Salary = 75000,
                DepartmentId = 1,
                Status = EmployeeStatus.Active,
                JobTitle = "Senior Developer"
            },
            new Employee
            {
                Id = 2,
                EmployeeNumber = "EMP002",
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@imas.com",
                PhoneNumber = "+1234567891",
                DateOfBirth = new DateTime(1990, 8, 22),
                HireDate = new DateTime(2021, 3, 10),
                Salary = 65000,
                DepartmentId = 2,
                Status = EmployeeStatus.Active,
                JobTitle = "HR Manager"
            }
        );
    }
}