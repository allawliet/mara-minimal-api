using imas.General.ApiService.Domain.Common;

namespace imas.General.ApiService.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ManagerId { get; set; }

    public bool IsActive() => !string.IsNullOrEmpty(Name);
    public bool HasManager() => ManagerId > 0;
}

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;

    public bool IsActive() => !string.IsNullOrEmpty(Name);
    public string[] GetPermissionList() => Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries);
}

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Industry { get; set; } = string.Empty;

    public bool IsActive() => !string.IsNullOrEmpty(Name);
    public bool HasContactInfo() => !string.IsNullOrEmpty(Phone) || !string.IsNullOrEmpty(Email);
}

public class Location : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public bool IsActive() => !string.IsNullOrEmpty(Name);
    public string GetFullAddress() => $"{Address}, {City}, {State} {PostalCode}, {Country}".Trim(new char[] { ' ', ',' });
}