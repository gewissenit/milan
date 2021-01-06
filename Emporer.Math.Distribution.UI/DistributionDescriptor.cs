#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using Emporer.Math.Distribution.Factories;

namespace Emporer.Math.Distribution.UI
{
  //TODO: this should be done by distconfvm-factories
  public class DistributionDescriptor
  {
    public DistributionDescriptor(IDistributionFactory<IRealDistribution> distributionFactory)
    {
      DistributionFactory = distributionFactory;
    }

    public IDistributionFactory<IRealDistribution> DistributionFactory { get; private set; }

    public string Name
    {
      get { return DistributionFactory.Name; }
    }

    public string Description
    {
      get { return DistributionFactory.Description; }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}