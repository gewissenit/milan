using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class PoissonDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Poisson"; }
    }

    public string Description
    {
      get { return "A Poisson distribution."; }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is PoissonDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new PoissonDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (PoissonDistributionConfiguration) cfg;
      return new PoissonDistributionConfiguration
             {
               Id = master.Id,
               Mean = master.Mean,
               ExpMean = master.ExpMean
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (PoissonDistributionConfiguration) cfg;
      return new PoissonDistribution
             {
               Mean = distConfig.Mean
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new PoissonDistribution();
    }
  }
}