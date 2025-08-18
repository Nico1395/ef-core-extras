namespace EfCoreExtras.Writes.Abstractions;

public interface INestedRemovingWritable<out TEntity, out TProperty> : INestedWritable<TEntity, TProperty>, IRemovingWritable<TEntity>
{
}
