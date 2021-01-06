#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;

namespace Milan.Simulation.Statistics
{
  public class HistoricalValueStore<TValue> : TimeAwareValueTracker<TValue>
  {
    private readonly Queue<TimedValue<TValue>> _valuesOverTime = new Queue<TimedValue<TValue>>();

    public HistoricalValueStore(Func<double> getCurrentTime)
      : base(getCurrentTime)
    {
    }

    public HistoricalValueStore(Func<TValue, double> toDouble, Func<double, TValue> fromDouble, Func<double> getCurrentTime)
      : base(toDouble, fromDouble, getCurrentTime)
    {
    }

    public Queue<TimedValue<TValue>> ValuesOverTime
    {
      get { return _valuesOverTime; }
    }

    public override void Update(TValue value)
    {
      base.Update(value);
      _valuesOverTime.Enqueue(new TimedValue<TValue>(GetCurrentTime(), value));
    }
  }
}