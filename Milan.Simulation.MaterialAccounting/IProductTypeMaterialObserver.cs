using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting
{
  public interface IProductTypeMaterialObserver<TEntity> : IMaterialObserver<TEntity>, IProductRelated
    where TEntity : IEntity
  {
  }
}