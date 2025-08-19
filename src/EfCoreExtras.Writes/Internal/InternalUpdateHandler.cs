using EfCoreExtras.Keys;
using EfCoreExtras.Writes.Abstractions;
using System.Collections;

namespace EfCoreExtras.Writes.Internal;

internal static class InternalUpdateHandler
{
    public static void HandleUpdating<TEntity, TProperty>(IWritable<TEntity> writable, TProperty? originalPropertyEntity, TProperty? updatedPropertyEntity)
        where TEntity : class
    {
        var propertyType = typeof(TProperty);
        if (propertyType.IsAssignableTo(typeof(IEnumerable)))
        {
            // Checking with 'is IEnumerable' is a null check as well as a cast
            if (updatedPropertyEntity is IEnumerable updatedEnumerable && originalPropertyEntity is not IEnumerable)   // The original collection is null so flag all 
            {
                foreach (var item in updatedEnumerable)
                    writable.Context.Add(item);
            }
            else if (updatedPropertyEntity is not IEnumerable && originalPropertyEntity is IEnumerable originalEnumerable)
            {
                foreach (var item in originalEnumerable)
                    writable.Context.Remove(item);
            }
            else if (updatedPropertyEntity is IEnumerable updated && originalPropertyEntity is IEnumerable original)
            {
                var updatedItems = updated.Cast<object>().ToList();
                var originalItems = original.Cast<object>().ToList();

                var encounteredOriginalItems = new List<object>();         // Collection of original items that were encountered. Original items that were not encountered should be deleted
                foreach (var updatedItem in updatedItems)
                {
                    if (updatedItem == null)
                        continue;

                    var originalItem = originalItems.SingleOrDefault(a => a != null && writable.Context.KeyValuesEqual(a, updatedItem));
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

                foreach (var missingItem in originalItems.Except(encounteredOriginalItems))
                {
                    if (missingItem != null)
                        writable.Context.Remove(missingItem);
                }
            }
        }
        else
        {
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
        }
    }
}
