namespace EfCoreExtras.Writes.Tests.Mocks.Factory;

internal static class PermissionFactory
{
    public static Permission Create()
    {
        var permission = new Permission
        {
            Id = Guid.NewGuid(),
            Name = "ReadData"
        };

        return permission;
    }
}
