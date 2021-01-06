#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;

namespace Milan.Simulation
{
  public interface IStationaryElement : IEntity, IReceiver<Product>, INode<IConnection>
  {
    IEnumerable<Product> ResidingProducts { get; }
  }
}