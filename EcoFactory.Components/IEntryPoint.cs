#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Emporer.Math.Distribution;
using Milan.Simulation;
using Milan.Simulation.ShiftSystems;

namespace EcoFactory.Components
{
  public interface IEntryPoint : IStationaryElement, IWorkingTimeDependent
  {
    IDistributionConfiguration BatchSize { get; set; }
    IRealDistribution BatchSizeDistribution { get; set; }
    IEnumerable<IProductTypeDistribution> ArrivalOccurrences { get; }
    IEnumerable<IProductTypeDistribution> BatchSizes { get; }
    IDictionary<IProductType, IRealDistribution> BatchesDictionary
    {
      get;
    }
    IDictionary<IProductType, IRealDistribution> ArrivalsDictionary
    {
      get;
    }
    void AddArrival(IProductTypeDistribution productTypeDistribution);
    void RemoveArrival(IProductTypeDistribution productTypeDistribution);
    void AddBatchSize(IProductTypeDistribution productTypeDistribution);
    void RemoveBatchSize(IProductTypeDistribution productTypeDistribution);
  }
}