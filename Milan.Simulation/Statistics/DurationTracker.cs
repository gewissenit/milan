#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class DurationTracker : HistoricalValueStore<TimeSpan>
  {
    public DurationTracker(Func<double> getCurrentTime)
      : base(SimulationTimeConversion.ToSimulationTimeSpan, SimulationTimeConversion.ToRealTimeSpan, getCurrentTime)
    {
    }
  }
}