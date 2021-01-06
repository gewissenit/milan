#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Observers
{
  public interface IProductTerminationCriteria : ITerminationCriteria
  {
    IEnumerable<IProductTypeAmount> ProductAmounts { get; }
    bool HasAndOperator { get; set; }
    void Add(IProductTypeAmount productAmount);
    void Remove(IProductTypeAmount productAmount);
  }
}