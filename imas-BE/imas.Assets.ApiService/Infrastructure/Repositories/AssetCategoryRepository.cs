using Microsoft.EntityFrameworkCore;
using imas.Assets.ApiService.Application.Interfaces;
using imas.Assets.ApiService.Domain.Entities;
using imas.Assets.ApiService.Infrastructure.Data;

namespace imas.Assets.ApiService.Infrastructure.Repositories;

public class AssetCategoryRepository : IAssetCategoryRepository
{
    private readonly AssetsDbContext _context;

    public AssetCategoryRepository(AssetsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AssetCategory>> GetAllAsync()
    {
        return await _context.AssetCategories
            .Include(c => c.Assets)
            .ToListAsync();
    }

    public async Task<AssetCategory?> GetByIdAsync(int id)
    {
        return await _context.AssetCategories
            .Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<AssetCategory?> GetByNameAsync(string name)
    {
        return await _context.AssetCategories
            .Include(c => c.Assets)
            .FirstOrDefaultAsync(c => c.Name == name);
    }

    public async Task<AssetCategory> CreateAsync(AssetCategory category)
    {
        _context.AssetCategories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<AssetCategory> UpdateAsync(AssetCategory category)
    {
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.AssetCategories.FindAsync(id);
        if (category != null)
        {
            _context.AssetCategories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.AssetCategories.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
    {
        var query = _context.AssetCategories.Where(c => c.Name == name);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);
        
        return await query.AnyAsync();
    }
}