using System.ComponentModel;

namespace EfCoreExtras.Writes.Abstractions.Internal;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IInternalNestedWritable<out TEntity, out TProperty> : IInternalWritable<TEntity>
{
    internal TProperty? PropertyEntity { get; }
    internal TProperty? OriginalPropertyEntity { get; }
}
