using EfCoreExtras.EntityTypes.TypeChecks;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace EfCoreExtras.Reads.Factories;

internal static class QueryableFactory
{
    public static IQueryable<TEntity> BuildIncludeQueryRecursively<TEntity>(IQueryable<TEntity> query, Type entityType, string prefix, int maxRecursionDepth, int currentRecursionDepth)
        where TEntity : class
    {
        if (currentRecursionDepth > maxRecursionDepth)
            return query;

        currentRecursionDepth++;

        var childPropeties = entityType.GetProperties().Where(p => EntityTypeValidator.IsValidChildType(p.PropertyType));
        foreach (var property in childPropeties)
        {
            var includePath = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
            query = query.Include(includePath);

            // Child collections are also reference types but contain properties that we dont want to iterate over, so dont recurse over their properties,
            // recurse over their element type instead to include those potentially related objects as well
            var isChildCollection = property.PropertyType.IsAssignableTo(typeof(IEnumerable)) && property.PropertyType != typeof(string);
            if (isChildCollection)
            {
                var elementType = property.PropertyType.GetGenericArguments()[0];
                query = BuildIncludeQueryRecursively(query, elementType, includePath, maxRecursionDepth, currentRecursionDepth);
            }
            else
            {
                query = BuildIncludeQueryRecursively(query, property.PropertyType, includePath, maxRecursionDepth, currentRecursionDepth);
            }
        }

        return query;
    }
}
