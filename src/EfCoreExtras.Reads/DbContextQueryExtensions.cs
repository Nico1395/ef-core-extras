using EfCoreExtras.EntityTypes.Keys;
using EfCoreExtras.EntityTypes.TypeChecks;
using EfCoreExtras.Reads.Factories;
using Microsoft.EntityFrameworkCore;

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
        var predicate = PredicateFactory.CreateRangedLookupPredicate(context, keyProperties, entities);

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
        var predicate = PredicateFactory.CreateRangedLookupPredicate(context, keyProperties, entities);

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
    public static TEntity? FindEager<TEntity>(this DbContext context, TEntity entity, int maxRecursionDepth = 10)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(entity);
        var keyValues = context.KeyValues(entity, keyProperties);
        var filterExpression = PredicateFactory.CreateLookupPredicate<TEntity>(keyProperties, keyValues);

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
    public static Task<TEntity?> FindEagerAsync<TEntity>(this DbContext context, TEntity entity, int maxRecursionDepth = 10, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(entity);
        var keyValues = context.KeyValues(entity, keyProperties);
        var filterExpression = PredicateFactory.CreateLookupPredicate<TEntity>(keyProperties, keyValues);

        return context
            .Set<TEntity>()
            .IncludeRecursively(maxRecursionDepth)
            .SingleOrDefaultAsync(filterExpression, cancellationToken);
    }

    /// <summary>
    /// Asynchronously finds an entity in the <see cref="DbContext"/> by its key values and includes related entities recursively.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="keyValues">Key values to search for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="Task{TResult}"/> representing the asynchronous operation, containing the found entity with related entities included, or <see langword="null"/>.</returns>
    public static Task<TEntity?> FindEagerAsync<TEntity>(this DbContext context, object?[] keyValues, int maxRecursionDepth = 10, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var filterExpression = PredicateFactory.CreateLookupPredicate<TEntity>(keyProperties, keyValues);

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
    public static List<TEntity> FindRangeEager<TEntity>(this DbContext context, IEnumerable<TEntity> entities, int maxRecursionDepth = 10)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var predicate = PredicateFactory.CreateRangedLookupPredicate(context, keyProperties, entities);

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
    public static async Task<List<TEntity>> FindRangeEagerAsync<TEntity>(this DbContext context, IEnumerable<TEntity> entities, int maxRecursionDepth = 10, CancellationToken cancellationToken = default)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var predicate = PredicateFactory.CreateRangedLookupPredicate(context, keyProperties, entities);

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
    public static IQueryable<TEntity> IncludeAll<TEntity>(this IQueryable<TEntity> query)
        where TEntity : class
    {
        foreach (var property in typeof(TEntity).GetProperties().Where(p => EntityTypeValidator.IsValidChildType(p.PropertyType)))
            query = query.Include(property.Name);

        return query;
    }

    /// <summary>
    /// Recursively includes related entities for the specified entity type in the query.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="query">The query to include related entities in.</param>
    /// <returns>The query with related entities recursively included.</returns>
    public static IQueryable<TEntity> IncludeRecursively<TEntity>(this IQueryable<TEntity> query, int maxRecursionDepth = 10)
        where TEntity : class
    {
        return QueryableFactory.BuildIncludeQueryRecursively(query, typeof(TEntity), "", maxRecursionDepth, 0);
    }
}
