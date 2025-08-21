using EfCoreExtras.EntityTypes.Keys;
using EfCoreExtras.Writes.Abstractions;
using EfCoreExtras.Writes.Abstractions.Internal.Implementations;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.Writes;

public static class DbContextWritableExtensions
{
    public static IAddingWritable<TEntity> StartAdd<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        context.Add(entity);
        return new Writable<TEntity>(context, entity);
    }

    public static IUpdatingWritable<TEntity> StartUpdate<TEntity>(this DbContext context, TEntity original, TEntity updated)
        where TEntity : class
    {
        context.SetValues(original, updated);
        return new Writable<TEntity>(context, original, updated);
    }

    public static IRemovingWritable<TEntity> StartRemove<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        context.Remove(entity);
        return new Writable<TEntity>(context, entity);
    }

    public static IIgnoringWritable<TEntity> StartIgnoring<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        context.Entry(entity).State = EntityState.Unchanged;
        return new Writable<TEntity>(context, entity);
    }
}
