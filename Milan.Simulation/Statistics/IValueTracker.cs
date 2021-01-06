#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Statistics
{
  public interface IValueTracker<TValue> : IStatisticalCounter<TValue>
  {
    TValue Minimum { get; }
    double MinimumNumeric { get; }
    TValue Maximum { get; }
    double MaximumNumeric { get; }
  }
}