using Milan.Simulation.Events;
using Milan.Simulation.Observers;

namespace Milan.Simulation.MaterialAccounting
{
  public interface IProcessMaterialObserver<TEntity, out TStartEvent, out TEndevent> : IProcessMaterialObserver<TEntity>, IProcessObserver<TEntity, TStartEvent, TEndevent>
    where TEntity : IEntity
    where TStartEvent : ISimulationEvent
    where TEndevent : ISimulationEvent
  {
  }

  public interface IProcessMaterialObserver<TEntity> : IMaterialObserver<TEntity>, IProcessObserver
    where TEntity : IEntity
  {
  }
}