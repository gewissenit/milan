#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Statistics
{
  public class StatisticalCounter<TValue> : IStatisticalCounter<TValue>
  {
    public virtual void Update(TValue value)
    {
      Count++;
      CurrentValue = value;
    }

    public int Count { get; private set; }

    public TValue CurrentValue { get; private set; }
  }
}