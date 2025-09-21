using Microsoft.EntityFrameworkCore;
using imas.Assets.ApiService.Domain.Entities;

namespace imas.Assets.ApiService.Infrastructure.Data;

public class AssetsDbContext : DbContext
{
    public AssetsDbContext(DbContextOptions<AssetsDbContext> options) : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetCategory> AssetCategories { get; set; }
    public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
    public DbSet<AssetAssignment> AssetAssignments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure table names
        modelBuilder.Entity<Asset>().ToTable("Assets");
        modelBuilder.Entity<AssetCategory>().ToTable("AssetCategories");
        modelBuilder.Entity<MaintenanceRecord>().ToTable("MaintenanceRecords");
        modelBuilder.Entity<AssetAssignment>().ToTable("AssetAssignments");

        // Configure Asset entity
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.SerialNumber).IsRequired().HasMaxLength(100);
            entity.Property(e => e.AssetTag).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.Manufacturer).HasMaxLength(100);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PurchasePrice).HasPrecision(18, 2);
            entity.Property(e => e.CurrentValue).HasPrecision(18, 2);

            entity.HasIndex(e => e.SerialNumber).IsUnique();
            entity.HasIndex(e => e.AssetTag).IsUnique();

            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Assets)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure AssetCategory entity
        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure MaintenanceRecord entity
        modelBuilder.Entity<MaintenanceRecord>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaintenanceType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Cost).HasPrecision(18, 2);

            entity.HasOne(e => e.Asset)
                  .WithMany(a => a.MaintenanceRecords)
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure AssetAssignment entity
        modelBuilder.Entity<AssetAssignment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Asset)
                  .WithMany(a => a.Assignments)
                  .HasForeignKey(e => e.AssetId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure default values and computed columns
        modelBuilder.Entity<Asset>()
                   .Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<Asset>()
                   .Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<AssetCategory>()
                   .Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<AssetCategory>()
                   .Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<MaintenanceRecord>()
                   .Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<MaintenanceRecord>()
                   .Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<AssetAssignment>()
                   .Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

        modelBuilder.Entity<AssetAssignment>()
                   .Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");
    }
}