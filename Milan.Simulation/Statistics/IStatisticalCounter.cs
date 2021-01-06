#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Statistics
{
  public interface IStatisticalCounter<TValue>
  {
    int Count { get; }
    TValue CurrentValue { get; }
    void Update(TValue value);
  }
}