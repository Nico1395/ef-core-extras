using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.EntityTypes.Keys.Abstractions;

public interface IEntityKeyFactory
{
    EntityKey Create(Type type, object item);
}

public interface IEntityKeyFactory<TContext> : IEntityKeyFactory
    where TContext : DbContext
{
}
