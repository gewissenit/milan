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
  public class ResourceTypeRetentionTimes
  {
    public ResourceTypeRetentionTimes(string entity, string resourceType, string resourcePool, Func<double> getCurrentTime)
    {
      Entity = entity;
      ResourceType = resourceType;
      ResourcePool = resourcePool;
      Values = new DurationTracker(getCurrentTime);
    }

    public string Entity { get; }
    public string ResourceType { get; }
    public string ResourcePool { get; }
    public DurationTracker Values { get; }

    public override string ToString()
    {
      return string.Format("{0}|{1} [{2}]", Entity, ResourceType, Values);
    }
  }
}