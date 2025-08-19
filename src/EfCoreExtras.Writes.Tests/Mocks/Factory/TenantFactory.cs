namespace EfCoreExtras.Writes.Tests.Mocks.Factory;

internal static class TenantFactory
{
    public static Tenant Create()
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "TestTenant"
        };

        return tenant;
    }
}
