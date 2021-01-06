#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Events
{
  public abstract class SimulationEvent : ISimulationEvent
  {
    private static long _eventCount; // for debugging purposes only

    public SimulationEvent(object sender, string eventName)
    {
      Id = _eventCount++;
      if (string.IsNullOrWhiteSpace(eventName))
      {
        throw new ArgumentException("The event name is not valid", nameof(eventName));
      }

      Name = eventName;
      Sender = sender;
    }

    public virtual ISimulationEvent RelatedEvent { get; protected set; }

    public long Id { get; }

    public virtual string Name { get; protected set; }
    
    public virtual void Handle()
    {
      OnOccur?.Invoke(this);
    }

    public double ScheduledTime { get; set; }
    public double InsertedTime { get; set; }
    public virtual object Sender { get; }
    public Action<ISimulationEvent> OnOccur { get; set; }

    public override string ToString()
    {
      return $"{Name} @ {ScheduledTime} ({Sender})";
    }
  }
}