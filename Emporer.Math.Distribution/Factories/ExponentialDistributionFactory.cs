using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class ExponentialDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Exponential"; }
    }

    public string Description
    {
      get { return "Simple negative-exponential distribution using the given average value (Mean parameter, must be >= 0."; }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is ExponentialDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new ExponentialDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (ExponentialDistributionConfiguration) cfg;
      return new ExponentialDistributionConfiguration
             {
               Id = master.Id,
               Mean = master.Mean,
               Minimum = master.Minimum
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (ExponentialDistributionConfiguration) cfg;
      return new ExponentialDistribution
             {
               Mean = distConfig.Mean,
               Minimum = distConfig.Minimum
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new ExponentialDistribution();
    }
  }
}