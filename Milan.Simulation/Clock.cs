#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation
{
  public class Clock : IClock
  {
    private readonly double _Epsilon;

    public Clock(double epsilon)
    {
      _Epsilon = epsilon;
    }

    public Clock()
      : this(double.Epsilon)
    {
    }

    public double Epsilon
    {
      get { return _Epsilon; }
    }

    public double CurrentTime { get; private set; }

    public void AdvanceTime(double timeDifference)
    {
      if (timeDifference == 0)
      {
        return;
      }
      if (timeDifference < 0)
      {
        throw new ArgumentException("Time difference must not be negative (cannot go back in time)!", "timeDifference");
      }
      if (Epsilon > timeDifference)
      {
        throw new ArgumentException(String.Format("Time difference is too small! Allowed: {0} but is: {1}", Epsilon, timeDifference), "timeDifference");
      }
      CurrentTime += timeDifference;
    }

    public void Reset(double startTime)
    {
      if (startTime < 0)
      {
        throw new ArgumentException("Start time must not be negative!", "startTime");
      }
      CurrentTime = startTime;
    }
  }
}