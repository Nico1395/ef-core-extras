namespace EfCoreExtras.Writes.Abstractions;

public interface INestedUpdatingWritable<out TEntity, out TProperty> : INestedWritable<TEntity, TProperty>, IUpdatingWritable<TEntity>
{
}
