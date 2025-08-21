using System.Collections.Concurrent;

namespace EfCoreExtras.EntityTypes.TypeChecks;

internal sealed record EntityTypeModelResult(Type EntityType, bool IsConfigured);

public static class EntityTypeValidator
{
    internal readonly static ConcurrentDictionary<Type, EntityTypeModelResult> _entityTypeResultCache = [];

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
