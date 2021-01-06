#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Math.Distribution;
using System.Collections.Generic;

namespace Milan.Simulation.Resources
{
  public interface IResourceType : IEntity
  {
    IDistributionConfiguration MaintenanceDuration
    {
      get;
      set;
    }
    IDistributionConfiguration UsageAmount
    {
      get;
      set;
    }

    IEnumerable<IResourceTypeInfluence> Influences { get; }

    void Add(IResourceTypeInfluence influence);
    void Remove(IResourceTypeInfluence influnence);
  }
}