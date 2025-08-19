namespace EfCoreExtras.Writes.Tests.Mocks.Factory;

internal static class RoleFactory
{
    public static Role Create(Tenant tenant, IEnumerable<Permission> permissions)
    {
        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            TenantId = tenant.Id,
            Tenant = tenant,
        };
        role.RolePermissions = permissions.Select(permission => new RolePermission()
        {
            RoleId = role.Id,
            PermissionId = permission.Id,
            Permission = permission
        }).ToList();

        return role;
    }
}
