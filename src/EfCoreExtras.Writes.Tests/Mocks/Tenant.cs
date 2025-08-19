namespace EfCoreExtras.Writes.Tests.Mocks;

public class Tenant
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}
