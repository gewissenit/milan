#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Emporer.Math.Distribution;
using Milan.JsonStore;
using Milan.Simulation;
using Milan.Simulation.Resources;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class TransformationRuleOutput : DomainEntity, ITransformationRuleOutput
  {
    [JsonProperty]
    private readonly IList<IProductTypeAmount> _Outputs = new List<IProductTypeAmount>();

    [JsonProperty]
    private readonly IList<IResourcePoolResourceTypeAmount> _resources = new List<IResourcePoolResourceTypeAmount>();

    public IRealDistribution Distribution { get; set; }

    public IEnumerable<IProductTypeAmount> Outputs
    {
      get { return _Outputs; }
    }

    public IEnumerable<IResourcePoolResourceTypeAmount> Resources
    {
      get { return _resources; }
    }

    [JsonProperty]
    public int Probability
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public IDistributionConfiguration ProcessingDuration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    public void Add(IProductTypeAmount output)
    {
      if (_Outputs.Any(cm => cm.ProductType == output.ProductType))
      {
        throw new InvalidOperationException("An identical product type already exists!");
      }
      _Outputs.Add(output);
    }

    public void Remove(IProductTypeAmount output)
    {
      if (!_Outputs.Contains(output))
      {
        throw new InvalidOperationException("The given input does not exist.");
      }
      _Outputs.Remove(output);
    }

    public void Add(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      if (_resources.Any(cm => cm.ResourceType == resourceResourceTypeAmount.ResourceType && cm.ResourcePool == resourceResourceTypeAmount.ResourcePool))
      {
        throw new InvalidOperationException("An identical resource type already exists!");
      }
      _resources.Add(resourceResourceTypeAmount);
    }

    public void Remove(IResourcePoolResourceTypeAmount resourceResourceTypeAmount)
    {
      if (!_resources.Contains(resourceResourceTypeAmount))
      {
        throw new InvalidOperationException("The given resourceType does not exist.");
      }
      _resources.Remove(resourceResourceTypeAmount);
    }
  }
}