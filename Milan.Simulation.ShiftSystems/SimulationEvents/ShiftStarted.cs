#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.ShiftSystems.SimulationEvents
{
  public class ShiftStarted : ShiftEvent
  {
    public const string EventName = "ShiftStarted";


    public ShiftStarted(IEntity sender, ShiftConfiguration shift)
      : base(sender, shift, EventName)
    {
    }
  }
}