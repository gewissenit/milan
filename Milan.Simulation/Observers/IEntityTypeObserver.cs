using System;

namespace Milan.Simulation.Observers
{
  public interface IEntityTypeObserver<out TEntity>: ISimulationObserver
    where TEntity : IEntity
  {
    Type EntityType { get; }
  }
}