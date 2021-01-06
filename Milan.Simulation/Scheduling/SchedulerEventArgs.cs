#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Events;

namespace Milan.Simulation.Scheduling
{
  public class SchedulerEventArgs : EventArgs
  {
    public SchedulerEventArgs(ISimulationEvent schedulable)
    {
      Schedulable = schedulable;
    }

    public ISimulationEvent Schedulable { get; private set; }
  }
}