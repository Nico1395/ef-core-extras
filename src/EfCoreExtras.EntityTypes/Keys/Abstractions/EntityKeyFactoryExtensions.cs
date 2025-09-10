namespace EfCoreExtras.EntityTypes.Keys.Abstractions;

public static class EntityKeyFactoryExtensions
{
    public static EntityKey GetKey(this IEntityKeyFactory factory, object item)
    {
        return factory.Create(item.GetType(), item);
    }
}
