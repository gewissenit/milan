#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion


using Milan.Simulation.Events;

namespace Milan.Simulation.Observers
{
  public interface IProcessObserver<TEntity, out TStartEvent, out TEndEvent> : IProcessObserver, IEntityObserver<TEntity>
    where TEntity : IEntity
    where TStartEvent : ISimulationEvent
    where TEndEvent : ISimulationEvent
  {
  }

  public interface IProcessObserver : ITimeReferenced, ISimulationObserver
  {
  }
}