#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System;
using System.Collections.Generic;

namespace Milan.Simulation.Resources
{
  public interface IResourcePool : IEntity
  {
    IEnumerable<IResourceTypeAmount> Resources { get; }
    IList<Resource> AvailableResources { get; }
    IEnumerable<Resource> AllResources { get; }
    void Add(IResourceTypeAmount resourceAmount);
    void Remove(IResourceTypeAmount resourceAmount);
    void Return(Resource resource);
    event EventHandler ResourcesReceived;
    bool HasAvailable(IResourceType resourceType, int amount);
    IEnumerable<Resource> GetResources(IResourceType resourceType, int amount);
  }
}