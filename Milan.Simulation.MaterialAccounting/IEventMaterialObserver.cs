
using Milan.Simulation.Events;
using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting
{
  public interface IEventMaterialObserver<TEntity> : IMaterialObserver<TEntity>
    where TEntity : IEntity
  {
  }

  public interface IEventMaterialObserver<TEntity, out TEvent> : IEventMaterialObserver<TEntity>, IEntityEventObserver<TEntity, TEvent>
    where TEntity : IEntity
    where TEvent : ISimulationEvent
  {
  }
}