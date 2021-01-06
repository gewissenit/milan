#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class TimeAwareValueTracker<TValue> : ValueAccumulator<TValue>
  {
    public TimeAwareValueTracker(Func<double> getCurrentTime)
    {
      if (getCurrentTime == null)
      {
        throw new ArgumentNullException();
      }

      GetCurrentTime = getCurrentTime;
      StartTime = GetCurrentTime();
    }

    public TimeAwareValueTracker(Func<TValue, double> toDouble, Func<double, TValue> fromDouble, Func<double> getCurrentTime)
      : base(toDouble, fromDouble)
    {
      if (getCurrentTime == null)
      {
        throw new ArgumentNullException();
      }
      GetCurrentTime = getCurrentTime;
      StartTime = GetCurrentTime();
    }

    public Func<double> GetCurrentTime { get; }


    public double PointInTimeOfMinimum { get; set; }
    public double PointInTimeOfMaximum { get; set; }
    public double StartTime { get; protected set; }
    public double LastUpdate { get; private set; }


    protected override void DoFirstUpdate()
    {
      base.DoFirstUpdate();
      LastUpdate = GetCurrentTime();
      PointInTimeOfMinimum = LastUpdate;
      PointInTimeOfMaximum = LastUpdate;
    }

    protected override void DoConsecutiveUpdate()
    {
      base.DoConsecutiveUpdate();
      LastUpdate = GetCurrentTime();
    }

    protected override void OnMinimumChanged()
    {
      base.OnMinimumChanged();
      PointInTimeOfMinimum = LastUpdate;
    }

    protected override void OnMaximumChanged()
    {
      base.OnMaximumChanged();
      PointInTimeOfMaximum = LastUpdate;
    }
  }
}