using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace EfCoreExtras.Writes.Abstractions.Internal;

[EditorBrowsable(EditorBrowsableState.Never)]
public interface IInternalWritable<out TEntity>
{
    internal TEntity Entity { get; }
    internal TEntity? Original { get; }
    internal DbContext Context { get; }
}
