using EfCoreExtras.Writes.Tests.DI;
using EfCoreExtras.Writes.Tests.Mocks;
using EfCoreExtras.Writes.Tests.Mocks.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfCoreExtras.Writes.Tests;

public class WritableTests
{
    private static DbContext CreateContext() => ServiceProviderFactory.Create().GetRequiredService<DbContext>();

    [Fact]
    public void Test()
    {
        var context = CreateContext();

        // The concept of this test is to mimic main data behavior.
        // Most of the times entities already exist in the database and are just added in some way as a dependency.
        // The point of the syntax is to allow developers to comfortably manage their writes without writing
        // overly verbose code that deals with this.

        var tenant = TenantFactory.Create();
        context.StartAdd(tenant).SaveChanges();

        var permission = PermissionFactory.Create();
        context.StartAdd(permission).SaveChanges();

        var role = RoleFactory.Create(tenant, [permission]);
        context.StartAdd(role)
            .Update(r => r.RolePermissions).ThenIgnore(r => r.Permission)
            .Ignore(r => r.Tenant)
            .SaveChanges();

        var user = UserFactory.Create(tenant, [role]);
        context.StartAdd(user)
            .Update(u => u.UserRoles).ThenIgnore(u => u.Role)
            .Ignore(u => u.Tenant)
            .SaveChanges();

        var queriedUser = context.Set<User>()
            .Include(u => u.UserRoles).ThenInclude(u => u.Role).ThenInclude(r => r.Tenant)
            .Include(u => u.UserRoles).ThenInclude(u => u.Role).ThenInclude(r => r.RolePermissions).ThenInclude(p => p.Permission)
            .Include(u => u.Tenant)
            .SingleOrDefault(u => u.Id == user.Id);

        Assert.NotNull(user);
        Assert.NotNull(user.Tenant);
        Assert.NotNull(user.UserRoles);
        Assert.NotEmpty(user.UserRoles);

        var queriedRole = user.UserRoles.SingleOrDefault()?.Role;
        Assert.NotNull(queriedRole);
        Assert.NotNull(queriedRole.Tenant);
        Assert.NotNull(queriedRole.RolePermissions);
        Assert.NotEmpty(queriedRole.RolePermissions);

        var queriedPermission = queriedRole.RolePermissions.SingleOrDefault()?.Permission;
        Assert.NotNull(queriedPermission);
    }
}
