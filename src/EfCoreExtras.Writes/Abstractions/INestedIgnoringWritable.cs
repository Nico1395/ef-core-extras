namespace EfCoreExtras.Writes.Abstractions;

public interface INestedIgnoringWritable<out TEntity, out TProperty> : INestedWritable<TEntity, TProperty>, IIgnoringWritable<TEntity>
{
}
