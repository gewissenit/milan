#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel;
using Emporer.Math.Distribution;

namespace Milan.Simulation
{
  public interface IProductTypeDistribution : INotifyPropertyChanged
  {
    IProductType ProductType { get; set; }
    IDistributionConfiguration DistributionConfiguration { get; set; }
  }
}