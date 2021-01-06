using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class GeometricDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Geometric"; }
    }

    public string Description
    {
      get
      {
        return
          "Discrete geometric distributed stream of pseudo random numbers of type integer. Values produced by this distribution are geometric determined by the probability.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is GeometricDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new GeometricDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (GeometricDistributionConfiguration) cfg;
      return new GeometricDistributionConfiguration
             {
               Id = master.Id,
               Probability = master.Probability
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (GeometricDistributionConfiguration) cfg;
      return new GeometricDistribution
             {
               Probability = distConfig.Probability
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new GeometricDistribution();
    }
  }
}