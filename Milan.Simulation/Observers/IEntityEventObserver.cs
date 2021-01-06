
using Milan.Simulation.Events;

namespace Milan.Simulation.Observers
{
  public interface IEntityEventObserver<TEntity, out TEvent> : IEntityObserver<TEntity>
    where TEntity : IEntity
    where TEvent : ISimulationEvent
  {
  }
}