#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Resources.Statistics
{
  public class ResourceRetentionTimeStatistic
  {
    public ResourceRetentionTimeStatistic(string entity,
                                       Resource resource,
                                       IEnumerable<int> counts,
                                       IEnumerable<TimeSpan> minDurations,
                                       IEnumerable<TimeSpan> avgDurations,
                                       IEnumerable<TimeSpan> maxDurations,
                                       IEnumerable<TimeSpan> sumDurations)
    {
      Entity = entity;
      Resource = resource;
      counts = counts.ToArray();
      MinCount = counts.Min();
      AvgCount = counts.Average();
      MaxCount = counts.Max();

      SumCounts = counts.Sum();
      MinDuration = GetAverageTimeSpan(minDurations);
      AvgDuration = GetAverageTimeSpan(avgDurations);
      MaxDuration = GetAverageTimeSpan(maxDurations);
      SumDuration = GetAverageTimeSpan(sumDurations);
    }

    public int SumCounts { get; private set; }

    public string Entity { get; private set; }
    public Resource Resource { get; private set; }

    public int MinCount { get; private set; }
    public double AvgCount { get; private set; }
    public int MaxCount { get; private set; }

    public TimeSpan MinDuration { get; private set; }
    public TimeSpan AvgDuration { get; private set; }
    public TimeSpan MaxDuration { get; private set; }
    public TimeSpan SumDuration { get; private set; }


    private TimeSpan GetAverageTimeSpan(IEnumerable<TimeSpan> timeSpans)
    {
      return TimeSpan.FromTicks(Convert.ToInt64(timeSpans.Average(ts => ts.Ticks)));
    }
  }
}