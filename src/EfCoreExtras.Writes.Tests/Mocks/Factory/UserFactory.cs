namespace EfCoreExtras.Writes.Tests.Mocks.Factory;

internal static class UserFactory
{
    public static User Create(Tenant tenant, IEnumerable<Role> roles)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            TenantId = tenant.Id,
            Tenant = tenant,
            UserRoles = []
        };
        user.UserRoles = roles.Select(role => new UserRole
        {
            RoleId = role.Id,
            Role = role,
            UserId = user.Id,
        }).ToList();

        return user;
    }
}
