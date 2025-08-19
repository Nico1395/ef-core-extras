using EfCoreExtras.Writes.Abstractions;
using EfCoreExtras.Writes.Abstractions.Internal.Implementations;
using EfCoreExtras.Writes.Internal;
using System.Linq.Expressions;

namespace EfCoreExtras.Writes;

public static class NestedWritableExtensions
{
    public static INestedAddingWritable<TEntity, TProperty> ThenAdd<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = writable.PropertyEntity != null ? navigationPropertyPath.Compile().Invoke(writable.PropertyEntity) : default;

        InternalAddHandler.HandleAdding(writable, propertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedAddingWritable<TEntity, IEnumerable<TProperty>> ThenAdd<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        if (writable.PropertyEntity == null)
            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null);

        var itemProperties = writable.PropertyEntity.Select(navigationFunc.Invoke).Where(p => p != null).Cast<TProperty>().ToList();
        InternalAddHandler.HandleAdding(writable, itemProperties);

        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, itemProperties);
    }

    public static INestedUpdatingWritable<TEntity, TProperty> ThenUpdate<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        var updatedPropertyEntity = writable.PropertyEntity != null ? navigationFunc.Invoke(writable.PropertyEntity) : default;
        var originalPropertyEntity = writable.OriginalPropertyEntity != null ? navigationFunc.Invoke(writable.OriginalPropertyEntity) : default;

        InternalUpdateHandler.HandleUpdating(writable, originalPropertyEntity, updatedPropertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, originalPropertyEntity, updatedPropertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TProperty>> ThenUpdate<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        var updatedItemProperties = writable.PropertyEntity?.Select(i => navigationFunc(i)).Where(i => i != null).Cast<TProperty>().ToList();
        var oritinalItemProperties = writable.OriginalPropertyEntity?.Select(i => navigationFunc(i)).Where(i => i != null).Cast<TProperty>().ToList();

        InternalUpdateHandler.HandleUpdating(writable, oritinalItemProperties, updatedItemProperties);
        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, oritinalItemProperties, updatedItemProperties);
    }

    public static INestedRemovingWritable<TEntity, TProperty> ThenRemove<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = writable.PropertyEntity != null ? navigationPropertyPath.Compile().Invoke(writable.PropertyEntity) : default;

        InternalRemoveHandler.HandleRemoving(writable, propertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedRemovingWritable<TEntity, IEnumerable<TProperty>> ThenRemove<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        if (writable.PropertyEntity == null)
            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null);

        var itemProperties = writable.PropertyEntity.Select(navigationFunc.Invoke).Where(p => p != null).Cast<TProperty>().ToList();
        InternalRemoveHandler.HandleRemoving(writable, itemProperties);

        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, itemProperties);
    }

    public static INestedIgnoringWritable<TEntity, TProperty> ThenIgnore<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = writable.PropertyEntity != null ? navigationPropertyPath.Compile().Invoke(writable.PropertyEntity) : default;

        InternalIgnoreHandler.HandleIgnoring(writable, propertyEntity);
        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedIgnoringWritable<TEntity, IEnumerable<TProperty>> ThenIgnore<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        if (writable.PropertyEntity == null)
            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null);

        var itemProperties = writable.PropertyEntity.Select(navigationFunc.Invoke).Where(p => p != null).Cast<TProperty>().ToList();
        InternalIgnoreHandler.HandleIgnoring(writable, itemProperties);

        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, itemProperties);
    }
}
