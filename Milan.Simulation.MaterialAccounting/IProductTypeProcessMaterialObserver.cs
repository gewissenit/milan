using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting
{
  public interface IProductTypeProcessMaterialObserver<TEntity> : IProcessObserver, IProductTypeMaterialObserver<TEntity>
    where TEntity : IEntity
  {
  }
}