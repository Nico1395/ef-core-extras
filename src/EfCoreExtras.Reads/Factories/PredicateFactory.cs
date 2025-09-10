using EfCoreExtras.EntityTypes.Keys;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace EfCoreExtras.Reads.Factories;

internal static class PredicateFactory
{
    public static Expression<Func<TEntity, bool>> CreateLookupPredicate<TEntity>(IReadOnlyList<IProperty> keyProperties, object?[] keyValues)
        where TEntity : class
    {
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

    public static Expression<Func<TEntity, bool>> CreateRangedLookupPredicate<TEntity>(DbContext context, IReadOnlyList<IProperty> keyProperties, IEnumerable<TEntity> entities)
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
}
