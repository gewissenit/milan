#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.Simulation.Statistics;

namespace Milan.Simulation.Resources
{
  public class Resource
  {
    private readonly IDictionary<IInfluence, HistoricalValueStore<double>> _influenceValues = new Dictionary<IInfluence, HistoricalValueStore<double>>();

    public Resource(IResourceType resourceType, IResourcePool resourcePool, Func<double> getCurrentTime)
    {
      ResourceType = resourceType;
      ResourcePool = resourcePool;
      foreach (var resourceTypeInfluence in resourceType.Influences)
      {
        var va = new HistoricalValueStore<double>(getCurrentTime);
        va.Update(resourceTypeInfluence.InitialValue);
        _influenceValues.Add(resourceTypeInfluence.Influence, va);
      }
    }

    public IResourceType ResourceType { get; }

    public IRealDistribution MaintenanceDuration { get; set; }
    public IRealDistribution UsageAmount { get; set; }
    public double CurrentUsageAmount { get; set; }

    public IReadOnlyDictionary<IInfluence, HistoricalValueStore<double>> InfluenceValues
    {
      get { return new ReadOnlyDictionary<IInfluence, HistoricalValueStore<double>>(_influenceValues); }
    }

    public Guid Id { get; } = Guid.NewGuid();

    public IResourcePool ResourcePool { get; }

    public bool IsAvailable
    {
      get
      {
        return InfluenceValues.All(iv => iv.Value.Sum <= ResourceType.Influences.Single(i => i.Influence == iv.Key)
                                                                     .UpperLimit);
      }
    }

    public void IncreaseInfluenceValue(IInfluence influence, double value, double timeSpan)
    {
      if (InfluenceValues.ContainsKey(influence))
      {
        var rti = ResourceType.Influences.Single(i => i.Influence == influence);
        _influenceValues[influence].Update(timeSpan * value * rti.IncreaseFactor);
      }
    }

    public void DecreaseInfluenceValues(double timeSpan)
    {
      foreach (var influenceValue in InfluenceValues)
      {
        var rti = ResourceType.Influences.Single(i => i.Influence == influenceValue.Key);
        var val = -1 * timeSpan * rti.RecoveryRate;
        if (influenceValue.Value.Sum + val < rti.LowerLimit)
        {
          val = influenceValue.Value.Sum - rti.LowerLimit;
        }
        _influenceValues[influenceValue.Key].Update(val);
      }
    }

    public override string ToString()
    {
      return Id.ToString();
    }
  }
}