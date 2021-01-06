#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using Milan.Simulation;
using Milan.Simulation.Events;
using Newtonsoft.Json;

namespace EcoFactory.Components
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ExitPoint : StationaryElement, IExitPoint
  {
    public override void Receive(Product product)
    {
      var exiting = new ThroughputEndEvent(this,
                                           new[]
                                           {
                                             product
                                           });
      exiting.Schedule(0);
    }


    public override bool IsAvailable(Product product)
    {
      return true;
    }

    protected override IEnumerable<Product> GetResidingProducts()
    {
      return new Product[0];
    }
  }
}