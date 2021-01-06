#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Milan.Simulation.Events
{
  public interface IProductsRelatedEvent : ISimulationEvent
  {
    IEnumerable<Product> Products { get; }
  }
}