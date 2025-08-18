using EfCoreExtras.Keys;
using EfCoreExtras.Writes.Abstractions;
using EfCoreExtras.Writes.Internal;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCoreExtras.Writes;

public static class WritableExtensions
{
    public static INestedUpdatingWritable<TEntity, TProperty> Add<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = navigationPropertyPath.Compile().Invoke(writable.Entity);
        if (propertyEntity != null)
            writable.Context.Add(propertyEntity);

        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TItem>> Add<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var enumerableProperty = navigationPropertyPath.Compile().Invoke(writable.Entity);
        if (enumerableProperty != null)
        {
            foreach (var item in enumerableProperty)
            {
                if (item != null)
                    writable.Context.Add(item);
            }
        }

        return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, enumerableProperty);
    }

    public static INestedUpdatingWritable<TEntity, TProperty> Update<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        var updatedPropertyEntity = navigationFunc.Invoke(writable.Entity);
        var originalPropertyEntity = writable.Original != null ? navigationFunc.Invoke(writable.Original) : default;

        if (writable.Original == null)
        {
            // If the original child is null but the updated one isnt, we add the updated one
            if (updatedPropertyEntity != null)
                writable.Context.Add(updatedPropertyEntity);
        }
        else
        {
            if (originalPropertyEntity != null)
            {
                if (updatedPropertyEntity != null)
                    writable.Context.Entry(originalPropertyEntity).CurrentValues.SetValues(updatedPropertyEntity);      // If both original and updated children are present, we simply update the values
                else
                    writable.Context.Remove(originalPropertyEntity);                                                    // If the original child is present but the updated one isnt, we remove the original
            }
        }

        return new NestedWritable<TEntity, TProperty>(writable, originalPropertyEntity, updatedPropertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TItem>> Update<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var collectionFunc = navigationPropertyPath.Compile();
        var updatedCollection = collectionFunc.Invoke(writable.Entity);
        var originalCollection = writable.Original != null ? collectionFunc.Invoke(writable.Original) : null;

        if (updatedCollection != null && originalCollection == null)   // The original collection is null so flag all 
        {
            foreach (var item in updatedCollection)
            {
                if (item != null)
                    writable.Context.Add(item);
            }

            return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, null, updatedCollection);
        }
        else if (updatedCollection == null && originalCollection != null)
        {
            foreach (var item in  originalCollection)
            {
                if (item != null)
                    writable.Context.Remove(item);
            }

            return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, originalCollection, null);
        }
        else if (updatedCollection != null && originalCollection != null)
        {
            var encounteredOriginalItems = new List<TItem>();         // Collection of original items that were encountered. Original items that were not encountered should be deleted
            foreach (var updatedItem in updatedCollection)
            {
                if (updatedItem == null)
                    continue;

                var originalItem = originalCollection.SingleOrDefault(a => a != null && writable.Context.KeyValuesEqual(a, updatedItem));
                if (originalItem == null)
                {
                    writable.Context.Add(updatedItem);
                }
                else
                {
                    writable.Context.Entry(originalItem).CurrentValues.SetValues(updatedItem);
                    encounteredOriginalItems.Add(originalItem);
                }
            }

            foreach (var missingItem in originalCollection.Except(encounteredOriginalItems))
            {
                if (missingItem != null)
                    writable.Context.Remove(missingItem);
            }

            return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, originalCollection, updatedCollection);
        }

        // Both collections have to be null at this point
        return new NestedWritable<TEntity, IEnumerable<TItem>>(writable, null, null);
    }

    public static INestedUpdatingWritable<TEntity, TProperty> Remove<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = navigationPropertyPath.Compile().Invoke(writable.Entity);
        if (propertyEntity != null)
            writable.Context.Remove(propertyEntity);

        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TItem>> Remove<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        throw new NotImplementedException();
    }

    public static INestedUpdatingWritable<TEntity, TProperty> Ignore<TEntity, TProperty>(this IWritable<TEntity> writable, Expression<Func<TEntity, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = navigationPropertyPath.Compile().Invoke(writable.Entity);
        if (propertyEntity != null)
            writable.Context.Entry(propertyEntity).State = EntityState.Unchanged;

        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedUpdatingWritable<TEntity, IEnumerable<TItem>> Ignore<TEntity, TItem>(this IWritable<TEntity> writable, Expression<Func<TEntity, IEnumerable<TItem>?>> navigationPropertyPath)
        where TEntity : class
    {
        var enumerableProperty = navigationPropertyPath.Compile().Invoke(writable.Entity);
        if (enumerableProperty != null)
        {
            foreach (var item in enumerableProperty)
            {
                if (item != null)
                    writable.Context.Entry(item).State = EntityState.Unchanged;
            }
        }

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
