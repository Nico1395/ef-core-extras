using EfCoreExtras.Writes.Tests.Mocks;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.Writes.Tests.Persistence;

internal sealed class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<UserRole>().HasKey(u => new { u.UserId, u.RoleId, });
        modelBuilder.Entity<Role>().HasKey(u => u.Id);
        modelBuilder.Entity<RolePermission>().HasKey(u => new { u.RoleId, u.PermissionId, });
        modelBuilder.Entity<Permission>().HasKey(u => u.Id);
        modelBuilder.Entity<Tenant>().HasKey(u => u.Id);
    }
}
