using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EfCoreExtras.EntityTypes.Keys;

/// <summary>
/// Provides extension methods for managing entity states in a <see cref="DbContext"/>.
/// </summary>
public static class DbContextEntryExtensions
{
    /// <summary>
    /// Checks if the specified entity is being tracked by the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity to check.</param>
    /// <returns><see langword="true"/> if the entity is being tracked; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entity"/> is <see langword="null"/>.</exception>
    public static bool Tracks<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        var entityKeyValues = context.KeyValues(entity, keyProperties);

        return context.ChangeTracker.Entries<TEntity>().Any(e =>
        {
            var keyValues = context.KeyValues(e.Entity, keyProperties);
            return context.KeyValuesEqual(keyValues, entityKeyValues);
        });
    }

    /// <summary>
    /// Checks if an entity with the specified key is being tracked by the <see cref="DbContext"/>.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entityKey">The key of the entity to check.</param>
    /// <returns><see langword="true"/> if the entity is being tracked; otherwise, <see langword="false"/>.</returns>
    public static bool Tracks<TEntity>(this DbContext context, EntityKey entityKey)
        where TEntity : class
    {
        var keyProperties = context.KeyProperties(typeof(TEntity));
        return context.ChangeTracker.Entries<TEntity>().Any(e =>
        {
            var keyValues = context.KeyValues(e.Entity, keyProperties);
            var key = EntityKey.FromValues(keyValues);

            return entityKey == key;
        });
    }

    /// <summary>
    /// Gets the tracked entry for the specified entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity to get the tracked entry for.</param>
    /// <returns>The tracked <see cref="EntityEntry{TEntity}"/> for the entity, or <see langword="null"/> if the entity is not being tracked.</returns>
    public static EntityEntry<TEntity>? TrackedEntry<TEntity>(this DbContext context, TEntity entity)
        where TEntity : class
    {
        var targetEntityType = entity.GetType();
        var keyProperties = context.KeyProperties(entity);
        var entityKeyValues = context.KeyValues(entity, keyProperties);

        return context.ChangeTracker.Entries<TEntity>().SingleOrDefault(e =>
        {
            if (e.Entity == entity)
                return true;
            else if (e.Entity.GetType() != targetEntityType.GetType())
                return false;

            var trackedEntityProperties = targetEntityType.GetProperties().Select(p => p.Name).ToList();
            var hasKeyProperties = keyProperties.All(p => trackedEntityProperties.Contains(p.Name));
            if (!hasKeyProperties)
                return false;

            var keyValues = context.KeyValues(e.Entity, keyProperties);
            return context.KeyValuesEqual(keyValues, entityKeyValues);
        });
    }

    /// <summary>
    /// Gets the tracked entry for an entity with the specified key values.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entityKeyValues">The key values of the entity to get the tracked entry for.</param>
    /// <returns>The tracked <see cref="EntityEntry{TEntity}"/> for the entity, or <see langword="null"/> if the entity is not being tracked.</returns>
    public static EntityEntry<TEntity>? TrackedEntry<TEntity>(this DbContext context, object?[] entityKeyValues)
        where TEntity : class
    {
        var targetEntityType = typeof(TEntity);
        var keyProperties = context.KeyProperties(targetEntityType);

        return context.ChangeTracker.Entries<TEntity>().SingleOrDefault(e =>
        {
            if (e.GetType() != targetEntityType)
                return false;

            var keyValues = context.KeyValues(e.Entity, keyProperties);
            return context.KeyValuesEqual(keyValues, entityKeyValues);
        });
    }

    /// <summary>
    /// Gets the tracked entry for an entity with the specified key.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entityKey">The key of the entity to get the tracked entry for.</param>
    /// <returns>The tracked <see cref="EntityEntry{TEntity}"/> for the entity, or <see langword="null"/> if the entity is not being tracked.</returns>
    public static EntityEntry<TEntity>? TrackedEntry<TEntity>(this DbContext context, EntityKey entityKey)
        where TEntity : class
    {
        var targetEntityType = typeof(TEntity);
        var keyProperties = context.KeyProperties(typeof(TEntity));

        return context.ChangeTracker.Entries<TEntity>().SingleOrDefault(e =>
        {
            if (e.GetType() != targetEntityType)
                return false;

            var keyValues = context.KeyValues(e.Entity, keyProperties);
            var key = EntityKey.FromValues(keyValues);

            return entityKey == key;
        });
    }

    /// <summary>
    /// Gets the key properties for the specified entity.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity to get the key properties for.</param>
    /// <returns>A read-only list of key properties for the entity.</returns>
    public static IReadOnlyList<IProperty> KeyProperties(this DbContext context, object entity)
    {
        return context.KeyProperties(entity.GetType());
    }

    /// <summary>
    /// Gets the key properties for the specified entity type.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entityType">The type of the entity to get the key properties for.</param>
    /// <returns>A read-only list of key properties for the entity type.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> or <paramref name="entityType"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">Thrown if the entity type is not part of the <see cref="DbContext"/> or does not have a primary key defined.</exception>
    public static IReadOnlyList<IProperty> KeyProperties(this DbContext context, Type entityType)
    {
        var modelEntityType = context.Model.FindEntityType(entityType) ?? throw new InvalidOperationException($"The type '{entityType.Name}' is not part of the DbContext '{context.GetType()}'.");
        var keyProperties = modelEntityType.FindPrimaryKey()?.Properties ?? throw new InvalidOperationException($"Entity {entityType.Name} does not have a primary key defined.");

        return keyProperties;
    }

    /// <summary>
    /// Gets the key values for the specified entity.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity to get the key values for.</param>
    /// <returns>An array of key values for the entity.</returns>
    public static object?[] KeyValues(this DbContext context, object entity)
    {
        var keyProperties = context.KeyProperties(entity);
        return context.KeyValues(entity, keyProperties);
    }

    /// <summary>
    /// Gets the key values for the specified entity and key properties.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity to get the key values for.</param>
    /// <param name="keyProperties">The key properties of the entity.</param>
    /// <returns>An array of key values for the entity.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/>, <paramref name="entity"/>, or <paramref name="keyProperties"/> is <see langword="null"/>.</exception>
    public static object?[] KeyValues(this DbContext context, object entity, IReadOnlyList<IProperty> keyProperties)
    {
        return keyProperties.Select(p => context.Entry(entity).Property(p.Name).CurrentValue).ToArray();
    }

    /// <summary>
    /// Compares two sets of key values for equality.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="leftKeyValues">The first set of key values.</param>
    /// <param name="rightKeyValues">The second set of key values.</param>
    /// <returns><see langword="true"/> if the key values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool KeyValuesEqual(this DbContext context, IEnumerable<object?> leftKeyValues, IEnumerable<object?> rightKeyValues)
    {
        return leftKeyValues.SequenceEqual(rightKeyValues);
    }

    /// <summary>
    /// Compares the key values of two entities for equality.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="leftEntity">The first entity.</param>
    /// <param name="rightEntity">The second entity.</param>
    /// <returns><see langword="true"/> if the key values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool KeyValuesEqual(this DbContext context, object leftEntity, object rightEntity)
    {
        var leftKeyValues = context.KeyValues(leftEntity);
        var rightKeyValues = context.KeyValues(rightEntity);

        return context.KeyValuesEqual(leftKeyValues, rightKeyValues);
    }

    /// <summary>
    /// Compares a set of key values with the key values of an entity for equality.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="leftKeyValues">The set of key values.</param>
    /// <param name="rightEntity">The entity to compare with.</param>
    /// <returns><see langword="true"/> if the key values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool KeyValuesEqual(this DbContext context, IEnumerable<object?> leftKeyValues, object rightEntity)
    {
        var rightKeyValues = context.KeyValues(rightEntity);
        return context.KeyValuesEqual(leftKeyValues, rightKeyValues);
    }

    /// <summary>
    /// Compares the key values of an entity with a set of key values for equality.
    /// </summary>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="leftEntity">The entity to compare with.</param>
    /// <param name="rightKeyValues">The set of key values.</param>
    /// <returns><see langword="true"/> if the key values are equal; otherwise, <see langword="false"/>.</returns>
    public static bool KeyValuesEqual(this DbContext context, object leftEntity, IEnumerable<object?> rightKeyValues)
    {
        var leftKeyValues = context.KeyValues(leftEntity);
        return context.KeyValuesEqual(leftKeyValues, rightKeyValues);
    }

    /// <summary>
    /// Sets the values of the existing entity to match the updated entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="context">The <see cref="DbContext"/> instance.</param>
    /// <param name="existingEntity">The existing entity to update.</param>
    /// <param name="updatedEntity">The updated entity with new values.</param>
    public static void SetValues<TEntity>(this DbContext context, TEntity existingEntity, TEntity updatedEntity)
        where TEntity : class
    {
        context.Entry(existingEntity).CurrentValues.SetValues(updatedEntity);
    }
}
