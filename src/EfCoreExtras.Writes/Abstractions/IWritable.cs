using EfCoreExtras.Writes.Abstractions.Internal;

namespace EfCoreExtras.Writes.Abstractions;

public interface IWritable<out TEntity> : IInternalWritable<TEntity>
{
}
