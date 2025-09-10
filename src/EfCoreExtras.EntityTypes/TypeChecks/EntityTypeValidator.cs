namespace EfCoreExtras.EntityTypes.TypeChecks;

public static class EntityTypeValidator
{
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
