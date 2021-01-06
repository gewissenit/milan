#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Unit;

namespace Milan.Simulation
{
  public static class SimulationTimeConversion
  {
    public static TimeSpan TicksToTimeSpan(this double ticks)
    {
      return TimeSpan.FromTicks(Convert.ToInt64(ticks));
    }

    public static DateTime ToRealDate(this double simulationTime, DateTime startTime)
    {
      return startTime.AddMilliseconds(simulationTime);
    }


    public static TimeSpan ToRealTimeSpan(this double simulationTimeSpan)
    {
      return TimeSpan.FromMilliseconds(simulationTimeSpan);
    }


    public static double ToSimulationTimeSpan(this TimeSpan simulationTimeSpan)
    {
      return simulationTimeSpan.TotalMilliseconds;
    }


    public static double ToSimulationTimeSpan(this double realTimeSpan, IUnit timeUnit)
    {
      if (Math.Abs(realTimeSpan - 0.0) < double.Epsilon)
      {
        return realTimeSpan;
      }
      switch (timeUnit.Symbol)
      {
        case "ms":
        {
          return realTimeSpan;
        }
        case "s":
        {
          return realTimeSpan * 1000;
        }
        case "min":
        {
          return realTimeSpan * 1000 * 60;
        }
        case "h":
        {
          return realTimeSpan * 1000 * 60 * 60;
        }
        case "d":
        {
          return realTimeSpan * 1000 * 60 * 60 * 24;
        }
        case "w":
        {
          return realTimeSpan * 1000 * 60 * 60 * 24 * 7;
        }
        default:
        {
          return 0.0;
        }
      }
    }


    public static double ToRealTimeSpan(this double simulationTimeSpan, IUnit timeUnit)
    {
      if (Math.Abs(simulationTimeSpan - 0.0) < double.Epsilon)
      {
        return simulationTimeSpan;
      }
      switch (timeUnit.Symbol)
      {
        case "ms":
        {
          return simulationTimeSpan;
        }
        case "s":
        {
          return simulationTimeSpan / 1000;
        }
        case "min":
        {
          return simulationTimeSpan / 1000 / 60;
        }
        case "h":
        {
          return simulationTimeSpan / 1000 / 60 / 60;
        }
        case "d":
        {
          return simulationTimeSpan / 1000 / 60 / 60 / 24;
        }
        case "w":
        {
          return simulationTimeSpan / 1000 / 60 / 60 / 24 / 7;
        }
        default:
        {
          return 0.0;
        }
      }
    }


    public static double ConvertTo(this TimeSpan source, IUnit targetUnit)
    {
      return source.ToSimulationTimeSpan()
                   .ToRealTimeSpan(targetUnit);
    }


    public static IUnit GetBestFittingTimeUnit(this TimeSpan timeSpan, IEnumerable<IUnit> timeUnits)
    {
      
      if (timeSpan.TotalDays >= 7)
      {
        return timeUnits.Single(u => u.Symbol == "w");
      }
      if (timeSpan.TotalDays >= 1)
      {
        return timeUnits.Single(u => u.Symbol == "d");
      }
      if (timeSpan.TotalHours >= 1)
      {
        return timeUnits.Single(u => u.Symbol == "h");
      }
      if (timeSpan.TotalMinutes >= 1)
      {
        return timeUnits.Single(u => u.Symbol == "min");
      }
      if (timeSpan.TotalSeconds >= 1)
      {
        return timeUnits.Single(u => u.Symbol == "s");
      }
      return timeUnits.Single(u => u.Symbol == "ms");
    }
  }
}