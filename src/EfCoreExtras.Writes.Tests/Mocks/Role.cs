namespace EfCoreExtras.Writes.Tests.Mocks;

public class Role
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public List<UserRole>? UserRoles { get; set; }
    public List<RolePermission>? RolePermissions { get; set; }
    public Tenant? Tenant { get; set; }
    public Guid TenantId { get; set; }
}
