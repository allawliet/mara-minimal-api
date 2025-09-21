using Microsoft.EntityFrameworkCore;
using imas.ApiService.Infrastructure.Models;
using imas.ApiService.Modules.Authentication;
using imas.ApiService.Modules.Todos;
using imas.ApiService.Modules.Weather;

namespace imas.ApiService.Infrastructure.Data;

/// <summary>
/// Shared Database Context for all IMAS microservices
/// This context includes all entities from all services to maintain a single database schema
/// </summary>
public class SharedImasDbContext : DbContext
{
    public SharedImasDbContext(DbContextOptions<SharedImasDbContext> options) : base(options)
    {
    }

    // Main API Service Entities
    public DbSet<User> Users { get; set; }
    public DbSet<Todo> Todos { get; set; }
    public DbSet<WeatherForecast> WeatherForecasts { get; set; }

    // HR Service Entities
    public DbSet<HREmployee> HREmployees { get; set; }
    public DbSet<HRDepartment> HRDepartments { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }
    public DbSet<PayrollRecord> PayrollRecords { get; set; }

    // Assets Service Entities
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetCategory> AssetCategories { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }

    // Finance Service Entities
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    // General Service Entities
    public DbSet<GeneralDepartment> GeneralDepartments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names to avoid conflicts
        ConfigureMainApiEntities(modelBuilder);
        ConfigureHREntities(modelBuilder);
        ConfigureAssetsEntities(modelBuilder);
        ConfigureFinanceEntities(modelBuilder);
        ConfigureGeneralEntities(modelBuilder);
    }

    private void ConfigureMainApiEntities(ModelBuilder modelBuilder)
    {
        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
        });

        // Configure Todo entity
        modelBuilder.Entity<Todo>(entity =>
        {
            entity.ToTable("Todos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        });

        // Configure WeatherForecast entity
        modelBuilder.Entity<WeatherForecast>(entity =>
        {
            entity.ToTable("WeatherForecasts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Summary).HasMaxLength(100);
        });
    }

    private void ConfigureHREntities(ModelBuilder modelBuilder)
    {
        // Configure HR Employee entity (renamed to avoid conflicts)
        modelBuilder.Entity<HREmployee>(entity =>
        {
            entity.ToTable("HR_Employees");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.EmployeeNumber).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure HR Department entity (renamed to avoid conflicts)
        modelBuilder.Entity<HRDepartment>(entity =>
        {
            entity.ToTable("HR_Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure LeaveRequest entity
        modelBuilder.Entity<LeaveRequest>(entity =>
        {
            entity.ToTable("HR_LeaveRequests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Reason).HasMaxLength(1000);
            entity.HasOne<HREmployee>()
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PayrollRecord entity
        modelBuilder.Entity<PayrollRecord>(entity =>
        {
            entity.ToTable("HR_PayrollRecords");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BasicSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Allowances).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Deductions).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NetSalary).HasColumnType("decimal(18,2)");
            entity.HasOne<HREmployee>()
                  .WithMany()
                  .HasForeignKey(e => e.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureAssetsEntities(ModelBuilder modelBuilder)
    {
        // Configure Asset entity
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.ToTable("Assets_Assets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AssetTag).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CurrentValue).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.AssetTag).IsUnique();
        });

        // Configure AssetCategory entity
        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.ToTable("Assets_Categories");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.DepreciationRate).HasColumnType("decimal(5,2)");
        });

        // Configure MaintenanceRecord entity
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.ToTable("Assets_MaintenanceRecords");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Cost).HasColumnType("decimal(18,2)");
            entity.HasOne<Asset>()
                  .WithMany()
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure AssetAssignment entity
        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.ToTable("Assets_Assignments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AssignedTo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(1000);
            entity.HasOne<Asset>()
                  .WithMany()
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureFinanceEntities(ModelBuilder modelBuilder)
    {
        // Configure Invoice entity
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Finance_Invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.InvoiceNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaidAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
            entity.HasIndex(e => e.InvoiceNumber).IsUnique();
        });

        // Configure Payment entity
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Finance_Payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Reference).HasMaxLength(100);
        });

        // Configure Budget entity
        modelBuilder.Entity<Budget>(entity =>
        {
            entity.ToTable("Finance_Budgets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.AllocatedAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SpentAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.RemainingAmount).HasColumnType("decimal(18,2)");
        });

        // Configure Expense entity
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("Finance_Expenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
        });
    }

    private void ConfigureGeneralEntities(ModelBuilder modelBuilder)
    {
        // Configure General Department entity (renamed to avoid conflicts)
        modelBuilder.Entity<GeneralDepartment>(entity =>
        {
            entity.ToTable("General_Departments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure Role entity
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("General_Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure Company entity
        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("General_Companies");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
        });

        // Configure Location entity
        modelBuilder.Entity<Location>(entity =>
        {
            entity.ToTable("General_Locations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
        });
    }
}