using EfCoreExtras.Writes.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.Writes.Internal;

internal sealed class Writable<TEntity> : IWritable<TEntity>, IAddingWritable<TEntity>, IUpdatingWritable<TEntity>, IRemovingWritable<TEntity>, IIgnoringWritable<TEntity>
{
    public Writable(DbContext context, TEntity entity)
    {
        Context = context;
        Entity = entity;
    }

    public Writable(DbContext context, TEntity original, TEntity updated)
    {
        Context = context;
        Original = original;
        Entity = updated;
    }

    public DbContext Context { get; }
    public TEntity Entity { get; }
    public TEntity? Original { get; }
}
