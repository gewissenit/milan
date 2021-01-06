#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation
{
  public class RoutedProductTransmitter : Transmitter<Product>
  {
    protected readonly IEnumerable<IConnection> _connections;

    public RoutedProductTransmitter(IEnumerable<IConnection> connections)
      : base(connections.Select(c => c.Destination))
    {
      _connections = connections;
    }

    public override bool CanTransmit(Product message)
    {
      var matchingDestinations = _connections.Where(r => r.ProductTypes.Contains(message.ProductType) || !r.IsRoutingPerProductType);
      return matchingDestinations.Any(r => r.Destination.IsAvailable(message));
    }

    protected override IReceiver<Product> SelectReveiver(Product message)
    {
      var matchingDestinations = _connections.Where(r => r.ProductTypes.Contains(message.ProductType) || !r.IsRoutingPerProductType);
      return matchingDestinations.Where(r => r.Destination.IsAvailable(message))
                                 .OrderBy(r => r.Priority)
                                 .First()
                                 .Destination;
    }
  }
}