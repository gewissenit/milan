#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.ComponentModel;

namespace Milan.Simulation
{
  public interface IConnection : INotifyPropertyChanged
  {
    IStationaryElement Destination { get; set; }
    int Priority { get; set; }
    bool IsRoutingPerProductType { get; set; }
    IEnumerable<IProductType> ProductTypes { get; }
    void Add(IProductType productType);
    void Remove(IProductType productType);
  }
}