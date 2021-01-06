#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Statistics
{
  public class TimedValue<TValue>
  {
    public TimedValue(double pointInTime, TValue value)
    {
      PointInTime = pointInTime;
      Value = value;
    }

    public double PointInTime { get; private set; }
    public TValue Value { get; private set; }
  }
}