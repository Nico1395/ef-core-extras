using EfCoreExtras.Writes.Abstractions;
using EfCoreExtras.Writes.Internal;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EfCoreExtras.Writes;

public static class NestedWritableExtensions
{
    public static INestedAddingWritable<TEntity, TProperty> ThenAdd<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = writable.PropertyEntity != null ? navigationPropertyPath.Compile().Invoke(writable.PropertyEntity) : default;
        if (propertyEntity != null)
            writable.Context.Add(propertyEntity);

        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedAddingWritable<TEntity, IEnumerable<TProperty>> ThenAdd<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        if (writable.PropertyEntity == null)
            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null);

        var itemProperties = new List<TProperty>();
        foreach (var item in writable.PropertyEntity)
        {
            var itemProperty = navigationFunc.Invoke(item);
            if (itemProperty != null)
            {
                writable.Context.Add(itemProperty);
                itemProperties.Add(itemProperty);
            }
        }

        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, itemProperties);
    }

    public static INestedUpdatingWritable<TEntity, TProperty> ThenUpdate<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        var updatedPropertyEntity = writable.PropertyEntity != null ? navigationFunc.Invoke(writable.PropertyEntity) : default;
        var originalPropertyEntity = writable.OriginalPropertyEntity != null ? navigationFunc.Invoke(writable.OriginalPropertyEntity) : default;

        if (writable.Original == null)
        {
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

    public static INestedUpdatingWritable<TEntity, IEnumerable<TProperty>> ThenUpdate<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        var updatedItemProperties = writable.PropertyEntity?.Select(i => navigationFunc(i)).Where(i => i != null).Cast<TProperty>().ToList();
        var oritinalItemProperties = writable.OriginalPropertyEntity?.Select(i => navigationFunc(i)).Where(i => i != null).Cast<TProperty>().ToList();

        if (updatedItemProperties != null && oritinalItemProperties == null)   // The original collection is null so flag all 
        {
            foreach (var itemProperty in updatedItemProperties)
                writable.Context.Add(itemProperty!);

            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null, updatedItemProperties);
        }
        else if (updatedItemProperties == null && oritinalItemProperties != null)
        {
            foreach (var item in oritinalItemProperties)
                writable.Context.Remove(item!);

            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, oritinalItemProperties, null);
        }
        else if (updatedItemProperties != null && oritinalItemProperties != null)
        {
            var encounteredOriginalItems = new List<TProperty>();         // Collection of original items that were encountered. Original items that were not encountered should be deleted
            foreach (var updatedItemProperty in updatedItemProperties)
            {
                if (updatedItemProperty == null)
                    continue;

                var originalItemProperty = oritinalItemProperties.SingleOrDefault(i => i != null && writable.Context.KeyValuesEqual(i, updatedItemProperty));
                if (originalItemProperty == null)
                {
                    if (updatedItemProperty != null)
                        writable.Context.Add(updatedItemProperty);
                }
                else
                {
                    encounteredOriginalItems.Add(originalItemProperty);

                    if (updatedItemProperty != null)
                        writable.Context.Entry(originalItemProperty).CurrentValues.SetValues(updatedItemProperty);
                    else
                        writable.Context.Remove(originalItemProperty);
                }
            }

            foreach (var missingItem in oritinalItemProperties.Except(encounteredOriginalItems))
                writable.Context.Remove(missingItem!);

            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, oritinalItemProperties, updatedItemProperties);
        }

        // Both collections have to be null at this point
        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null, null);
    }

    public static INestedRemovingWritable<TEntity, TProperty> ThenRemove<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = writable.PropertyEntity != null ? navigationPropertyPath.Compile().Invoke(writable.PropertyEntity) : default;
        if (propertyEntity != null)
            writable.Context.Remove(propertyEntity);

        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedRemovingWritable<TEntity, IEnumerable<TProperty>> ThenRemove<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        if (writable.PropertyEntity == null)
            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null);

        var itemProperties = new List<TProperty>();
        foreach (var item in writable.PropertyEntity)
        {
            var itemProperty = navigationFunc.Invoke(item);
            if (itemProperty != null)
            {
                writable.Context.Remove(itemProperty);
                itemProperties.Add(itemProperty);
            }
        }

        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, itemProperties);
    }

    public static INestedIgnoringWritable<TEntity, TProperty> ThenIgnore<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, TPreviousProperty> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var propertyEntity = writable.PropertyEntity != null ? navigationPropertyPath.Compile().Invoke(writable.PropertyEntity) : default;
        if (propertyEntity != null)
            writable.Context.Entry(propertyEntity).State = EntityState.Unchanged;

        return new NestedWritable<TEntity, TProperty>(writable, propertyEntity);
    }

    public static INestedIgnoringWritable<TEntity, IEnumerable<TProperty>> ThenIgnore<TEntity, TPreviousProperty, TProperty>(this INestedWritable<TEntity, IEnumerable<TPreviousProperty>> writable, Expression<Func<TPreviousProperty, TProperty?>> navigationPropertyPath)
        where TEntity : class
    {
        var navigationFunc = navigationPropertyPath.Compile();
        if (writable.PropertyEntity == null)
            return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, null);

        var itemProperties = new List<TProperty>();
        foreach (var item in writable.PropertyEntity)
        {
            var itemProperty = navigationFunc.Invoke(item);
            if (itemProperty != null)
            {
                writable.Context.Entry(itemProperty).State = EntityState.Unchanged;
                itemProperties.Add(itemProperty);
            }
        }

        return new NestedWritable<TEntity, IEnumerable<TProperty>>(writable, itemProperties);
    }
}
