using EfCoreExtras.Writes.Abstractions;
using System.Collections;

namespace EfCoreExtras.Writes.Internal;

internal static class InternalRemoveHandler
{
    public static void HandleRemoving<TEntity, TProperty>(IWritable<TEntity> writable, TProperty? propertyEntity)
        where TEntity : class
    {
        if (propertyEntity == null)
            return;

        if (propertyEntity is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                writable.Context.Remove(item);
        }
        else
        {
            writable.Context.Remove(propertyEntity);
        }
    }
}
