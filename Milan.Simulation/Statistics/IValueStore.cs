#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Milan.Simulation.Statistics
{
  public interface IValueStore<TValue> : IValueAccumulator<TValue>
  {
    IEnumerable<TimedValue<TValue>> ValuesOverTime { get; }
  }
}