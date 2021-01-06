#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion


using Milan.Simulation.Events;

namespace Milan.Simulation.Observers
{
  public abstract class EntityEventObserver<TEntity, TEvent> : EntityObserver<TEntity>, IEntityEventObserver<TEntity, TEvent>
    where TEntity : class, IEntity
    where TEvent : class, ISimulationEvent
  {
    protected override void OnEntityEventOccurred(ISimulationEvent e)
    {
      var observedEvent = e as TEvent;

      if (observedEvent == null)
      {
        return;
      }
      OnEntityEventOccurred(observedEvent);
    }


    protected abstract void OnEntityEventOccurred(TEvent observedEvent);
  }
}