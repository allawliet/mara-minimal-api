using Microsoft.EntityFrameworkCore;
using imas.General.ApiService.Domain.Entities;

namespace imas.General.ApiService.Infrastructure.Data;

public class GeneralDbContext : DbContext
{
    public GeneralDbContext(DbContextOptions<GeneralDbContext> options) : base(options)
    {
    }

    public DbSet<Department> Departments { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Location> Locations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>().ToTable("Departments");
        modelBuilder.Entity<Role>().ToTable("Roles");
        modelBuilder.Entity<Company>().ToTable("Companies");
        modelBuilder.Entity<Location>().ToTable("Locations");
    }
}