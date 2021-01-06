#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Milan.JsonStore;

namespace Emporer.Math.Distribution
{
  public abstract class DistributionConfiguration : DomainEntity, IDistributionConfiguration
  {
    public string Name { get; protected set; }
  }
}