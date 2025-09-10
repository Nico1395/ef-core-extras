using Microsoft.EntityFrameworkCore;

namespace EfCoreExtras.EntityTypes.Keys.Abstractions;

/// <summary>
/// Base class providing a full implementation for the <see cref="IEntityKeyFactory"/> and <see cref="IEntityKeyFactory{TContext}"/> interfaces.
/// </summary>
/// <remarks>
/// Depending on your needs you can inherit from this abstract base class and register it generically or non-generically. This is done so when
/// having multiple <see cref="DbContext"/> implementations with different models, you can comfortably implement and register an <see cref="IEntityKeyFactory{TContext}"/>
/// for every context with this base class. If thats not needed, just register with the single <see cref="DbContext"/> implementation you've got.
/// </remarks>
/// <typeparam name="TContext">Type of context the factory uses as a source for key information.</typeparam>
/// <param name="_context"><see cref="DbContext"/> to source the key information from.</param>
public abstract class EntityKeyFactory<TContext>(TContext _context) : IEntityKeyFactory, IEntityKeyFactory<TContext>
    where TContext : DbContext
{
    public EntityKey Create(Type type, object item)
    {
        var keyValues = _context.KeyValues(item);
        if (keyValues.Any(v => v == null))
            throw new NullReferenceException($"An {nameof(EntityKey)} cannot have any of its parts be 'null'.");

        return EntityKey.FromValues(values: keyValues.Cast<object>().ToArray());
    }
}
