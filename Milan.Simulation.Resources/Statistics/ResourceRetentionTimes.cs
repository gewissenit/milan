#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using Milan.Simulation.Statistics;

namespace Milan.Simulation.Resources.Statistics
{
  /// <summary>
  ///   Provides statistics about how often and how long a specific resource remained inside a specific stationary
  ///   element.
  /// </summary>
  public class ResourceRetentionTimes
  {
    public ResourceRetentionTimes(string entity, Resource resource, Func<double> getCurrentTime)
    {
      Entity = entity;
      Resource = resource;
      Values = new DurationTracker(getCurrentTime);
    }

    public string Entity { get; private set; }
    public Resource Resource { get; private set; }
    public DurationTracker Values { get; private set; }

    public override string ToString()
    {
      return $"{Entity}|{Resource} [{Values}]";
    }
  }
}