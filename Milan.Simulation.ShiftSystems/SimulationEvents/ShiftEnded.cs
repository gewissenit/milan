#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

using Milan.Simulation.Events;

namespace Milan.Simulation.ShiftSystems.SimulationEvents
{
  /// <summary>
  ///   This simulation event occurs when a shift has ended.
  /// </summary>
  public class ShiftEnded : ShiftEvent
  {
    private const string EventName = "ShiftEnded";


    /// <exception cref="NullReferenceException"><c>NullReferenceException</c>.</exception>
    public ShiftEnded(IEntity sender, ShiftConfiguration shift, ISimulationEvent startEvent)
      : base(sender, shift, EventName)
    {
      if (startEvent == null)
      {
        throw new ArgumentNullException("startEvent");
      }
      RelatedEvent = startEvent;
    }
  }
}