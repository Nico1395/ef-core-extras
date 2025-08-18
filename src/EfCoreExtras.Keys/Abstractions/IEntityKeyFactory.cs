using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.Keys.Abstractions;

public interface IEntityKeyFactory
{
    EntityKey Create(Type type, object item);
}

public interface IEntityKeyFactory<TContext> : IEntityKeyFactory
    where TContext : DbContext
{
}
