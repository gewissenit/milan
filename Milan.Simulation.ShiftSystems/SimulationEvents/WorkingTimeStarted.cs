#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion


using Milan.Simulation.Events;

namespace Milan.Simulation.ShiftSystems.SimulationEvents
{
  /// <summary>
  ///   This simulation event occurs when a shift has started and no other shift was previously active.
  /// </summary>
  public class WorkingTimeStarted : SimulationEvent
  {
    private const string EventName = "WorkingTimeStarted";


    /// <summary>
    ///   Initializes a new instance of the <see cref="WorkingTimeStarted" /> class.
    /// </summary>
    /// <param name="sender">The sender.</param>
    public WorkingTimeStarted(IEntity sender)
      : base(sender, EventName)
    {
    }
  }
}