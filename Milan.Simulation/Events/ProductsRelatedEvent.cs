#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation.Events
{
  public abstract class ProductsRelatedEvent : SimulationEvent, IProductsRelatedEvent
  {
    public ProductsRelatedEvent(IEntity sender, string eventName, IEnumerable<Product> relatedProducts)
      : base(sender, eventName)
    {
      if (!relatedProducts.Any())
      {
        throw new InvalidOperationException();
      }
      Products = relatedProducts;
    }


    public virtual IEnumerable<Product> Products { get; private set; }
  }
}