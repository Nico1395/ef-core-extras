using EfCoreExtras.Writes.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EfCoreExtras.Writes.Internal;

internal static class InternalIgnoreHandler
{
    public static void HandleIgnoring<TEntity, TProperty>(IWritable<TEntity> writable, TProperty? propertyEntity)
        where TEntity : class
    {
        if (propertyEntity == null)
            return;

        if (propertyEntity is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                writable.Context.Entry(item).State = EntityState.Unchanged;
        }
        else
        {
            writable.Context.Entry(propertyEntity).State = EntityState.Unchanged;
        }
    }
}
