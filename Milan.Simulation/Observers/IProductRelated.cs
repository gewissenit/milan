#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

namespace Milan.Simulation.Observers
{
  public interface IProductRelated
  {
    bool IsProductTypeSpecific { get; set; }

    IProductType ProductType { get; set; }

    QuantityReference QuantityReference { get; set; }
  }
}