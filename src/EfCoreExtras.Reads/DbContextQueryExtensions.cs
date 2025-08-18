using EfCoreExtras.Keys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections;
using System.Linq.Expressions;

namespace EfCoreExtras.Reads;

/// <summary>
/// Provides extension methods for querying entities in a <see cref="DbContext"/>, including finding entities by key values
/// and recursively including related entities.
/// </summary>
public static class DbContextQueryExtensions
{
    /// <summary>
    /// Finds an entity in the <see cref="DbContext"/> by its key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity with key values to search for.</param>
    /// <returns>The found entity, or <see langword="null"/> if no entity is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
    public static TEntity? Find<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        var keyValues = context.KeyValues(entity);
        return context.Set<TEntity>().Find(keyValues);
    }

    /// <summary>
    /// Asynchronously finds an entity in the <see cref="DbContext"/> by its key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity with key values to search for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ValueTask{TResult}"/> representing the asynchronous operation, containing the found entity or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
    public static ValueTask<TEntity?> FindAsync<TEntity>(this DbContext context, TEntity entity, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyValues = context.KeyValues(entity);
        return context.Set<TEntity>().FindAsync(keyValues, cancellationToken);
    }

    /// <summary>
    /// Finds multiple entities in the <see cref="DbContext"/> by their key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entities">The entities with key values to search for.</param>
    /// <returns>A list of found entities.</returns>
    public static List<TEntity> FindRange<TEntity>(this DbContext context, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var predicate = CreateCombinedKeyPredicate(context, keyProperties, entities);

        return context
            .Set<TEntity>()
            .Where(predicate)
            .ToList();
    }

    /// <summary>
    /// Asynchronously finds multiple entities in the <see cref="DbContext"/> by their key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entities">The entities with key values to search for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a list of found entities.</returns>
    public static async Task<List<TEntity>> FindRangeAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var predicate = CreateCombinedKeyPredicate(context, keyProperties, entities);

        return await context
            .Set<TEntity>()
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Finds an entity in the <see cref="DbContext"/> by its key values and includes related entities recursively.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity with key values to search for.</param>
    /// <returns>The found entity with related entities included, or <see langword="null"/> if no entity is found.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
    public static TEntity? FindEager<TEntity>(this DbContext context, TEntity entity, int maxRecursionDepth = 10)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(entity);
        var filterExpression = CreateKeyPredicate(context, keyProperties, entity);

        return context
            .Set<TEntity>()
            .IncludeRecursively(maxRecursionDepth)
            .SingleOrDefault(filterExpression);
    }

    /// <summary>
    /// Asynchronously finds an entity in the <see cref="DbContext"/> by its key values and includes related entities recursively.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity with key values to search for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the found entity with related entities included, or <see langword="null"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
    public static Task<TEntity?> FindEagerAsync<TEntity>(this DbContext context, TEntity entity, int maxRecursionDepth = 10, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(entity);
        var filterExpression = CreateKeyPredicate(context, keyProperties, entity);

        return context
            .Set<TEntity>()
            .IncludeRecursively(maxRecursionDepth)
            .SingleOrDefaultAsync(filterExpression, cancellationToken);
    }

    /// <summary>
    /// Finds multiple entities in the <see cref="DbContext"/> by their key values and includes related entities recursively.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entities">The entities with key values to search for.</param>
    /// <returns>A list of found entities with related entities included.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entities"/> is <see langword="null"/>.</exception>
    public static List<TEntity> FindRangeEager<TEntity>(this DbContext context, IEnumerable<TEntity> entities, int maxRecursionDepth = 10)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var predicate = CreateCombinedKeyPredicate(context, keyProperties, entities);

        return context
            .Set<TEntity>()
            .IncludeRecursively(maxRecursionDepth)
            .Where(predicate)
            .ToList();
    }

    /// <summary>
    /// Asynchronously finds multiple entities in the <see cref="DbContext"/> by their key values and includes related entities recursively.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entities.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entities">The entities with key values to search for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing a list of found entities with related entities included.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entities"/> is <see langword="null"/>.</exception>
    public static async Task<List<TEntity>> FindRangeEagerAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entities, int maxRecursionDepth = 10, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var predicate = CreateCombinedKeyPredicate(context, keyProperties, entities);

        return await context
            .Set<TEntity>()
            .IncludeRecursively(maxRecursionDepth)
            .Where(predicate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Includes all related entities for the specified entity type in the query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="query">The query to include related entities in.</param>
    /// <returns>The query with all related entities included.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
    public static IQueryable<TEntity> IncludeAll<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class
    {
        foreach (var property in typeof(TEntity).GetProperties().Where(p => IsValidChildType(p.PropertyType)))
            query = query.Include(property.Name);

        return query;
    }

    /// <summary>
    /// Recursively includes related entities for the specified entity type in the query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="query">The query to include related entities in.</param>
    /// <returns>The query with related entities recursively included.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="query"/> is <see langword="null"/>.</exception>
    public static IQueryable<TEntity> IncludeRecursively<TEntity>(this IQueryable<TEntity> query, int maxRecursionDepth = 10)
        where TEntity : class
    {
        return IncludeRecursivelyInternal(query, typeof(TEntity), "", maxRecursionDepth, 0);
    }

    private static IQueryable<TEntity> IncludeRecursivelyInternal<TEntity>(IQueryable<TEntity> query, Type entityType, string prefix, int maxRecursionDepth, int recursionDepth)
        where TEntity : class
    {
        if (recursionDepth > maxRecursionDepth)
            return query;

        recursionDepth++;

        foreach (var property in entityType.GetProperties().Where(p => IsValidChildType(p.PropertyType)))
        {
            var includePath = string.IsNullOrEmpty(prefix) ? property.Name : $"{prefix}.{property.Name}";
            query = query.Include(includePath);

            // Child collections are also reference types but contain properties that we dont want to iterate over, so dont recurse over their properties,
            // recurse over their element type instead to include those potential related objects as well
            var isChildCollection = property.PropertyType.IsAssignableTo(typeof(IEnumerable)) && property.PropertyType != typeof(string);
            if (isChildCollection)
            {
                var elementType = property.PropertyType.GetGenericArguments()[0];
                query = IncludeRecursivelyInternal(query, elementType, includePath, maxRecursionDepth, recursionDepth);
            }
            else
            {
                query = IncludeRecursivelyInternal(query, property.PropertyType, includePath, maxRecursionDepth, recursionDepth);
            }
        }

        return query;
    }

    private static Expression<Func<TEntity, bool>> CreateKeyPredicate<TEntity>(DbContext context, IReadOnlyList<IProperty> keyProperties, TEntity entity)
        where TEntity : class
    {
        var keyValues = context.KeyValues(entity, keyProperties);
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        Expression? body = null;

        for (int i = 0; i < keyProperties.Count; i++)
        {
            var keyProperty = keyProperties[i];
            var keyValue = Expression.Constant(keyValues[i], keyProperty.ClrType);

            var propertyAccess = Expression.Property(parameter, keyProperty.Name);
            var equalsExpression = Expression.Equal(propertyAccess, keyValue);

            body = body == null ? equalsExpression : Expression.AndAlso(body, equalsExpression);
        }

        return Expression.Lambda<Func<TEntity, bool>>(body!, parameter);
    }

    private static Expression<Func<TEntity, bool>> CreateCombinedKeyPredicate<TEntity>(DbContext context, IReadOnlyList<IProperty> keyProperties, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        var entityKeys = entities.Select(entity => context.KeyValues(entity, keyProperties));
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var keyExpressions = entityKeys.Select(keys =>
        {
            Expression? keyMatch = null;
            for (int i = 0; i < keyProperties.Count; i++)
            {
                var propertyAccess = Expression.Property(parameter, keyProperties[i].Name);
                var keyValue = Expression.Constant(keys[i], keyProperties[i].ClrType);
                var equalsExpression = Expression.Equal(propertyAccess, keyValue);

                keyMatch = keyMatch == null ? equalsExpression : Expression.AndAlso(keyMatch, equalsExpression);
            }
            return keyMatch!;
        });

        var combinedExpression = keyExpressions.Aggregate(Expression.OrElse);
        return Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);
    }

    /// <summary>
    /// Determines whether the specified type is a child entity type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the specified type is a child entity type; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidChildType(Type type)
    {
        return type.IsClass && type != typeof(string);
    }
}
