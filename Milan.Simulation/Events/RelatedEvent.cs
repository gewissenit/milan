#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Events
{
  public abstract class RelatedEvent : SimulationEvent, IRelatedEvent
  {
    /// <exception cref="ArgumentNullException"><paramref name="relatedStartEvent" /> is <c>null</c>.</exception>
    public RelatedEvent(IEntity sender, string eventName, ISimulationEvent relatedStartEvent)
      : base(sender, eventName)
    {
      if (relatedStartEvent == null)
      {
        throw new ArgumentNullException(nameof(relatedStartEvent));
      }
      RelatedEvent = relatedStartEvent;
    }


    public virtual double Duration => ScheduledTime - RelatedEvent.ScheduledTime;
  }
}