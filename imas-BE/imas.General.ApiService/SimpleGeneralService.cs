using Microsoft.EntityFrameworkCore;

namespace imas.General.ApiService;

public class SimpleGeneralService
{
    private readonly GeneralDbContext _context;

    public SimpleGeneralService(GeneralDbContext context)
    {
        _context = context;
    }

    // Department methods
    public async Task<IEnumerable<object>> GetAllDepartmentsAsync()
    {
        var departments = await _context.Departments.ToListAsync();
        return departments.Select(d => new
        {
            d.Id,
            d.Name,
            d.Description,
            d.ManagerId,
            d.CreatedAt
        });
    }

    public async Task<object?> GetDepartmentByIdAsync(int id)
    {
        var department = await _context.Departments.FindAsync(id);
        if (department == null) return null;

        return new
        {
            department.Id,
            department.Name,
            department.Description,
            department.ManagerId,
            department.CreatedAt
        };
    }

    // Role methods
    public async Task<IEnumerable<object>> GetAllRolesAsync()
    {
        var roles = await _context.Roles.ToListAsync();
        return roles.Select(r => new
        {
            r.Id,
            r.Name,
            r.Description,
            r.Permissions,
            r.CreatedAt
        });
    }

    public async Task<object?> GetRoleByIdAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return null;

        return new
        {
            role.Id,
            role.Name,
            role.Description,
            role.Permissions,
            role.CreatedAt
        };
    }

    // Company methods
    public async Task<IEnumerable<object>> GetAllCompaniesAsync()
    {
        var companies = await _context.Companies.ToListAsync();
        return companies.Select(c => new
        {
            c.Id,
            c.Name,
            c.Description,
            c.Address,
            c.Phone,
            c.Email,
            c.Industry,
            c.CreatedAt
        });
    }

    public async Task<object?> GetCompanyByIdAsync(int id)
    {
        var company = await _context.Companies.FindAsync(id);
        if (company == null) return null;

        return new
        {
            company.Id,
            company.Name,
            company.Description,
            company.Address,
            company.Phone,
            company.Email,
            company.Industry,
            company.CreatedAt
        };
    }

    // Location methods
    public async Task<IEnumerable<object>> GetAllLocationsAsync()
    {
        var locations = await _context.Locations.ToListAsync();
        return locations.Select(l => new
        {
            l.Id,
            l.Name,
            l.Address,
            l.City,
            l.State,
            l.Country,
            l.PostalCode,
            l.CreatedAt
        });
    }

    public async Task<object?> GetLocationByIdAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null) return null;

        return new
        {
            location.Id,
            location.Name,
            location.Address,
            location.City,
            location.State,
            location.Country,
            location.PostalCode,
            location.CreatedAt
        };
    }

    // Customer methods  
    public async Task<IEnumerable<object>> GetAllCustomersAsync()
    {
        var customers = await _context.Customers.ToListAsync();
        return customers.Select(c => new
        {
            c.Id,
            c.Name,
            c.Email,
            c.Phone,
            c.Address,
            c.CreatedAt
        });
    }

    public async Task<object?> GetCustomerByIdAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer == null) return null;

        return new
        {
            customer.Id,
            customer.Name,
            customer.Email,
            customer.Phone,
            customer.Address,
            customer.CreatedAt
        };
    }

    // Vendor methods
    public async Task<IEnumerable<object>> GetAllVendorsAsync()
    {
        var vendors = await _context.Vendors.ToListAsync();
        return vendors.Select(v => new
        {
            v.Id,
            v.Name,
            v.ContactEmail,
            v.ContactPhone,
            v.Address,
            v.CreatedAt
        });
    }

    public async Task<object?> GetVendorByIdAsync(int id)
    {
        var vendor = await _context.Vendors.FindAsync(id);
        if (vendor == null) return null;

        return new
        {
            vendor.Id,
            vendor.Name,
            vendor.ContactEmail,
            vendor.ContactPhone,
            vendor.Address,
            vendor.CreatedAt
        };
    }

    // Document methods
    public async Task<IEnumerable<object>> GetAllDocumentsAsync()
    {
        var documents = await _context.Documents.ToListAsync();
        return documents.Select(d => new
        {
            d.Id,
            d.Title,
            d.Description,
            d.FilePath,
            d.FileSize,
            d.ContentType,
            d.UploadedBy,
            d.CreatedAt
        });
    }

    public async Task<object?> GetDocumentByIdAsync(int id)
    {
        var document = await _context.Documents.FindAsync(id);
        if (document == null) return null;

        return new
        {
            document.Id,
            document.Title,
            document.Description,
            document.FilePath,
            document.FileSize,
            document.ContentType,
            document.UploadedBy,
            document.CreatedAt
        };
    }
}