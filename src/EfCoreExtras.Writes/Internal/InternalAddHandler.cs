using EfCoreExtras.Writes.Abstractions;
using System.Collections;

namespace EfCoreExtras.Writes.Internal;

internal static class InternalAddHandler
{
    public static void HandleAdding<TEntity, TProperty>(IWritable<TEntity> writable, TProperty propertyEntity)
        where TEntity : class
    {
        if (propertyEntity == null)
            return;

        if (propertyEntity is IEnumerable enumerableProperty)
        {
            foreach (var item in enumerableProperty)
                writable.Context.Add(item);
        }
        else
        {
            writable.Context.Add(propertyEntity);
        }
    }
}
