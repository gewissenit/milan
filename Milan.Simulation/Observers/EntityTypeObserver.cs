#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

using Milan.Simulation.Events;

namespace Milan.Simulation.Observers
{
  public abstract class EntityTypeObserver<TEntity>: SchedulerObserver, IEntityTypeObserver<TEntity>
    where TEntity : IEntity
  {
    public Type EntityType
    {
      get { return typeof (TEntity); }
    }

    protected abstract void OnEntityTypeEventOccurred(ISimulationEvent e);

    protected override void OnEventOccurred(ISimulationEvent e)
    {
      if (EntityType.IsInstanceOfType(e.Sender))
      {
        OnEntityTypeEventOccurred(e);
      }
    }
  }
}