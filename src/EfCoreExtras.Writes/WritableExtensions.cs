using EfCoreExtras.Writes.Abstractions;
using EfCoreExtras.Writes.Abstractions.Internal.Implementations;
using EfCoreExtras.Writes.Internal;
using System.Linq.Expressions;

namespace EfCoreExtras.Writes;

public static class WritableExtensions
{
    public static INestedAddingWritable<TEntity, TProperty> Add<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = navigationPropertyPath.Compile().Invoke(writable.Entity);

        InternalAddHandler.HandleAdding(writable, propertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TItem>> Add<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var enumerableProperty = navigationPropertyPath.Compile().Invoke(writable.Entity);

        InternalAddHandler.HandleAdding(writable, enumerableProperty);
        return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, enumerableProperty);
    }

    public static INestedAddingWritable<TEntity, TProperty> Update<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        var updatedPropertyEntity = navigationFunc.Invoke(writable.Entity);
        var originalPropertyEntity = writable.Original != null ? navigationFunc.Invoke(writable.Original) : default;

        InternalUpdateHandler.HandleUpdating(writable, originalPropertyEntity, updatedPropertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, originalPropertyEntity, updatedPropertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TItem>> Update<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var collectionFunc = navigationPropertyPath.Compile();
        var updatedCollection = collectionFunc.Invoke(writable.Entity);
        var originalCollection = writable.Original != null ? collectionFunc.Invoke(writable.Original) : null;

        InternalUpdateHandler.HandleUpdating(writable, originalCollection, updatedCollection);
        return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, null, null);
    }

    public static INestedRemovingWritable<TEntity, TProperty> Remove<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = navigationPropertyPath.Compile().Invoke(writable.Entity);

        InternalRemoveHandler.HandleRemoving(writable, propertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedRemovingWritable<TEntity, IEnumerable<TItem>> Remove<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var enumerableProperty = navigationPropertyPath.Compile().Invoke(writable.Entity);

        InternalRemoveHandler.HandleRemoving(writable, enumerableProperty);
        return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, enumerableProperty);
    }

    public static INestedIgnoringWritable<TEntity, TProperty> Ignore<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = navigationPropertyPath.Compile().Invoke(writable.Entity);

        InternalIgnoreHandler.HandleIgnoring(writable, propertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedIgnoringWritable<TEntity, IEnumerable<TItem>> Ignore<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var enumerableProperty = navigationPropertyPath.Compile().Invoke(writable.Entity);

        InternalIgnoreHandler.HandleIgnoring(writable, enumerableProperty);
        return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, enumerableProperty);
    }

    public static int SaveChanges<TEntity>(this IWritable<TEntity> writable)
        where TEntity : class
    {
        return writable.Context.SaveChanges();
    }

    public static Task<int> SaveChangesAsync<TEntity>(this IWritable<TEntity> writable, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return writable.Context.SaveChangesAsync(cancellationToken);
    }
}
