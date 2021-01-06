#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.Resources;

namespace EcoFactory.Components
{
  public interface ITransformationRuleOutput : INotifyPropertyChanged
  {
    int Probability { get; set; }
    IDistributionConfiguration ProcessingDuration { get; set; }
    IEnumerable<IProductTypeAmount> Outputs { get; }
    IEnumerable<IResourcePoolResourceTypeAmount> Resources { get; }
    IRealDistribution Distribution { get; set; }
    void Add(IProductTypeAmount productTypeAmount);
    void Remove(IProductTypeAmount productTypeAmount);
    void Add(IResourcePoolResourceTypeAmount resourceResourceTypeAmount);
    void Remove(IResourcePoolResourceTypeAmount resourceResourceTypeAmount);
  }
}