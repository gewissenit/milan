#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;

namespace Milan.Simulation.Statistics
{
  public class ProcessStatistic
  {
    public ProcessStatistic(string entity, string process, string productType, Func<double> getCurrentTime)
    {
      Entity = entity;
      Process = process;
      ProductType = productType;
      Durations = new DurationTracker(getCurrentTime);
      RelatedProducts = new ValueAccumulator<int>();
    }

    public string Entity { get; private set; }
    public string Process { get; private set; }
    public string ProductType { get; private set; }
    public DurationTracker Durations { get; private set; }
    public ValueAccumulator<int> RelatedProducts { get; private set; }

    public override string ToString()
    {
      return string.Format("{0}|{1}|{2} [{3}]", Entity, Process, ProductType, Durations);
    }
  }

  public class UnfinishedProcess
  {
    public UnfinishedProcess(string entity, string process, string[] productTypes, double remainingDuration, double elapsedDuration)
    {
      Entity = entity;
      Process = process;
      ProductTypes = productTypes;
      RemainingDuration = remainingDuration;
      ElapsedDuration = elapsedDuration;
    }

    public string Entity { get; }
    public string Process { get; }
    public string[] ProductTypes { get; private set; }
    public double RemainingDuration { get; }
    public double ElapsedDuration { get; }


    public override string ToString()
    {
      return string.Format("Unfinished {0} at {1} ({2}) [{3} -> {4}]", Entity, Process, "none", ElapsedDuration, RemainingDuration);
    }
  }
}