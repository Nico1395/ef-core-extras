namespace EfCoreExtras.Writes.Tests.Mocks;

public class Permission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public List<RolePermission>? RolePermissions { get; set; }
}
