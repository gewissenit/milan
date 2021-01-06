#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

using Milan.Simulation.Events;

namespace Milan.Simulation.ShiftSystems.SimulationEvents
{
  /// <summary>
  ///   A base class for shift related simulation events.
  /// </summary>
  public abstract class ShiftEvent : SimulationEvent
  {
    /// <summary>
    ///   Initializes a new instance of the <see cref="ShiftEvent" /> class.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="shift">The shift.</param>
    /// <param name="name">The name.</param>
    protected ShiftEvent(IEntity sender, ShiftConfiguration shift, string name)
      : base(sender, name)
    {
      if (shift == null)
      {
        throw new ArgumentNullException("shift");
      }
      Shift = shift;
    }


    /// <summary>
    ///   Gets the shift.
    /// </summary>
    public ShiftConfiguration Shift { get; private set; }
  }
}