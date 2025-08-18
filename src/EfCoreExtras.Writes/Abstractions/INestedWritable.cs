using EfCoreExtras.Writes.Abstractions.Internal;

namespace EfCoreExtras.Writes.Abstractions;

public interface INestedWritable<out TEntity, out TProperty> : IWritable<TEntity>, IInternalNestedWritable<TEntity, TProperty>
{
}
