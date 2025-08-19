namespace EfCoreExtras.Writes.Tests.Mocks;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
    public List<UserRole>? UserRoles { get; set; }
    public Tenant? Tenant { get; set; }
    public Guid TenantId { get; set; }
}
