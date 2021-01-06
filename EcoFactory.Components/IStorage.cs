#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation;

namespace EcoFactory.Components
{
  public interface IStorage : IStationaryElement
  {
    bool HasCapacityPerProductType { get; set; }
    bool HasLimitedCapacity { get; set; }
    int Capacity { get; set; }
    int Count { get; }
    IEnumerable<IProductTypeAmount> ProductTypeCounts { get; }
    IEnumerable<IProductTypeAmount> ProductTypeCapacities { get; }
    int GetCount(IProductType productType);
    void AddProductTypeCapacity(IProductTypeAmount productTypeCapacity);
    void RemoveProductTypeCapacity(IProductTypeAmount productTypeCapacity);
  }
}