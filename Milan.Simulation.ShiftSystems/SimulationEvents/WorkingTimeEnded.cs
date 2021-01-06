#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion


using Milan.Simulation.Events;

namespace Milan.Simulation.ShiftSystems.SimulationEvents
{
  /// <summary>
  ///   This simulation event occurs when a shift has ended and no other shift is active or will start immediately.
  /// </summary>
  public class WorkingTimeEnded : SimulationEvent
  {
    private const string EventName = "WorkingTimeEnded";


    /// <summary>
    ///   Initializes a new instance of the <see cref="WorkingTimeEnded" /> class.
    /// </summary>
    /// <param name="sender">The sender.</param>
    public WorkingTimeEnded(IEntity sender)
      : base(sender, EventName)
    {
    }
  }
}