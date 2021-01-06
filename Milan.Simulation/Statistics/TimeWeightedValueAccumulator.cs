#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class TimeWeightedValueAccumulator<TValue> : TimeAwareValueTracker<TValue>
  {
    public TimeWeightedValueAccumulator(Func<double> getCurrentTime)
      : base(getCurrentTime)
    {
    }
    
    public override void Update(TValue value)
    {
      var timeDifference = GetCurrentTime() - LastUpdate;
      var currentValue = ToDouble(CurrentValue);
      SumNumeric += currentValue * timeDifference;
      SquareSumNumeric += currentValue * currentValue * timeDifference;
      base.Update(value);
    }
  }
}