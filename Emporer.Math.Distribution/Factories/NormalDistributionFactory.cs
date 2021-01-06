#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class NormalDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Normal"; }
    }

    public string Description
    {
      get
      {
        return
          "Normal (also known as \"Gaussian\") distribution. It is a family of distributions of the same general form, differing in their location and scale parameters: the mean (\"average\") and standard deviation (\"variability\"), respectively.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is NormalDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new NormalDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (NormalDistributionConfiguration) cfg;
      return new NormalDistributionConfiguration
             {
               Id = master.Id,
               Mean = master.Mean,
               StandardDeviation = master.StandardDeviation
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (NormalDistributionConfiguration) cfg;
      return new NormalDistribution
             {
               Mean = distConfig.Mean,
               StandardDeviation = distConfig.StandardDeviation
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new NormalDistribution();
    }
  }
}