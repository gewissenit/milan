using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class LogNormalDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Log Normal"; }
    }

    public string Description
    {
      get
      {
        return
          "The Lognormal-distribution with average value (Mean parameter) and variance (Variance parameter) is a Normal-distribution, shaping the natural logarithm of the characteristic values ln(variableValue) instead of variableValue-values.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is LogNormalDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new LogNormalDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (LogNormalDistributionConfiguration) cfg;
      return new LogNormalDistributionConfiguration
             {
               Id = master.Id,
               Minimum = master.Minimum,
               Mean = master.Mean,
               StandardDeviation = master.StandardDeviation,
               ParamsNormal = master.ParamsNormal
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (LogNormalDistributionConfiguration) cfg;
      return new LogNormalDistribution
             {
               ParamsNormal = distConfig.ParamsNormal,
               Minimum = distConfig.Minimum,
               Mean = distConfig.Mean,
               StandardDeviation = distConfig.StandardDeviation
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new LogNormalDistribution();
    }
  }
}