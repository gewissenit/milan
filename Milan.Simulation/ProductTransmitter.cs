#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Linq;

namespace Milan.Simulation
{
  public class ProductTransmitter : RoutedProductTransmitter
  {
    //private IConnection _currentConnection;

    public ProductTransmitter(IEnumerable<IConnection> connections)
      : base(connections)
    {
    }

    //public override bool CanTransmit(Product message)
    //{
    //  if (_currentConnection == null ||
    //      (!_currentConnection.ProductTypes.Contains(message.ProductType) && _currentConnection.IsRoutingPerProductType) ||
    //      !_currentConnection.Destination.IsAvailable(message))
    //  {
    //    return base.CanTransmit(message);
    //  }
    //  return true;
    //}

    //protected override IReceiver<Product> SelectReveiver(Product message)
    //{
    //  if (_currentConnection == null ||
    //      (!_currentConnection.ProductTypes.Contains(message.ProductType) && _currentConnection.IsRoutingPerProductType) ||
    //      !_currentConnection.Destination.IsAvailable(message))
    //  {
    //    var matchingDestinations = _connections.Where(r => r.ProductTypes.Contains(message.ProductType) || !r.IsRoutingPerProductType);
    //    _currentConnection = matchingDestinations.Where(r => r.Destination.IsAvailable(message))
    //                                              .OrderBy(r => r.Priority)
    //                                              .First();
    //  }
    //  return _currentConnection.Destination;
    //}
  }
}