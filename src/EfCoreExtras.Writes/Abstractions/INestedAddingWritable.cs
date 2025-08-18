namespace EfCoreExtras.Writes.Abstractions;

public interface INestedAddingWritable<out TEntity, out TProperty> : INestedWritable<TEntity ,TProperty>, IAddingWritable<TEntity>
{
}
