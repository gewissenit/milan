#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Events;
using Milan.Simulation.Observers;

namespace Milan.Simulation.CostAccounting
{
  public interface IEventCostObserver : ICostObserver
  {
  }

  public interface IEventCostObserver<TEntity, out TEvent> : IEventCostObserver, IEntityEventObserver<TEntity, TEvent>
    where TEntity : IEntity
    where TEvent : ISimulationEvent
  {
  }
}