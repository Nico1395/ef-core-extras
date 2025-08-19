using EfCoreExtras.Writes.Tests.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EfCoreExtras.Writes.Tests.DI;

internal static class ServiceProviderFactory
{
    public static IServiceProvider Create()
    {
        var services = new ServiceCollection();

        services.AddDbContext<DbContext, TestDbContext>(options =>
        {
            options.UseInMemoryDatabase("ef-core-extras-writes");
        });

        return services.BuildServiceProvider();
    }
}
