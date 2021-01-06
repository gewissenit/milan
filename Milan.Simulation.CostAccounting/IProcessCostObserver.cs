#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.Simulation.Events;
using Milan.Simulation.Observers;

namespace Milan.Simulation.CostAccounting
{
  public interface IProcessCostObserver<TEntity> : ICostObserver<TEntity>, IProcessObserver
    where TEntity : IEntity
  {
  }

  public interface IProcessCostObserver<TEntity, out TStartEvent, out TEndevent> : IProcessCostObserver<TEntity>, IProcessObserver<TEntity, TStartEvent, TEndevent>
    where TEntity : IEntity
    where TStartEvent : ISimulationEvent
    where TEndevent : ISimulationEvent
  {
  }
}