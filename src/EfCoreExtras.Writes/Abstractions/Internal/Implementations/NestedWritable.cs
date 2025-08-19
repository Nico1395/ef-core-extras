using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.Writes.Abstractions.Internal.Implementations;

internal sealed class NestedWritable<TEntity, TProperty> : INestedAddingWritable<TEntity, TProperty>, INestedUpdatingWritable<TEntity, TProperty>, INestedRemovingWritable<TEntity, TProperty>, INestedIgnoringWritable<TEntity, TProperty>
{
    public NestedWritable(IWritable<TEntity> root, TProperty? propertyEntity)
    {
        Context = root.Context;
        Original = root.Original;
        Entity = root.Entity;
        PropertyEntity = propertyEntity;
    }

    public NestedWritable(IWritable<TEntity> root, TProperty? originalPropertyEntity, TProperty? updatedPropertyEntity)
    {
        Context = root.Context;
        Original = root.Original;
        Entity = root.Entity;
        OriginalPropertyEntity = originalPropertyEntity;
        PropertyEntity = updatedPropertyEntity;
    }

    public DbContext Context { get; }
    public TEntity Entity { get; }
    public TEntity? Original { get; }
    public TProperty? PropertyEntity { get; }
    public TProperty? OriginalPropertyEntity { get; }
}
