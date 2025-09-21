using Microsoft.EntityFrameworkCore;
using imas.Assets.ApiService.Application.Interfaces;
using imas.Assets.ApiService.Domain.Entities;
using imas.Assets.ApiService.Infrastructure.Data;

namespace imas.Assets.ApiService.Infrastructure.Repositories;

public class AssetRepository : IAssetRepository
{
    private readonly AssetsDbContext _context;

    public AssetRepository(AssetsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Asset>> GetAllAsync()
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.MaintenanceRecords)
            .Include(a => a.Assignments)
            .ToListAsync();
    }

    public async Task<Asset?> GetByIdAsync(int id)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.MaintenanceRecords)
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Asset?> GetBySerialNumberAsync(string serialNumber)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.MaintenanceRecords)
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.SerialNumber == serialNumber);
    }

    public async Task<Asset?> GetByAssetTagAsync(string assetTag)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.MaintenanceRecords)
            .Include(a => a.Assignments)
            .FirstOrDefaultAsync(a => a.AssetTag == assetTag);
    }

    public async Task<IEnumerable<Asset>> GetByCategoryAsync(int categoryId)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.MaintenanceRecords)
            .Include(a => a.Assignments)
            .Where(a => a.CategoryId == categoryId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Asset>> GetByStatusAsync(string status)
    {
        return await _context.Assets
            .Include(a => a.Category)
            .Include(a => a.MaintenanceRecords)
            .Include(a => a.Assignments)
            .Where(a => a.Status == status)
            .ToListAsync();
    }

    public async Task<Asset> CreateAsync(Asset asset)
    {
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task<Asset> UpdateAsync(Asset asset)
    {
        _context.Entry(asset).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task DeleteAsync(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset != null)
        {
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Assets.AnyAsync(a => a.Id == id);
    }

    public async Task<bool> SerialNumberExistsAsync(string serialNumber, int? excludeId = null)
    {
        var query = _context.Assets.Where(a => a.SerialNumber == serialNumber);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        
        return await query.AnyAsync();
    }

    public async Task<bool> AssetTagExistsAsync(string assetTag, int? excludeId = null)
    {
        var query = _context.Assets.Where(a => a.AssetTag == assetTag);
        if (excludeId.HasValue)
            query = query.Where(a => a.Id != excludeId.Value);
        
        return await query.AnyAsync();
    }
}