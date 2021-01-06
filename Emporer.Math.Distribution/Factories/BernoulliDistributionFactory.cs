using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IBoolDistribution>))]
  internal class BernoulliDistributionFactory : IDistributionFactory<IBoolDistribution>
  {
    public string Name
    {
      get { return "Bernoulli"; }
    }

    public string Description
    {
      get { return "A Bernoulli distribution, which returning true values with a given probability."; }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is BernoulliDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new BernoulliDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (BernoulliDistributionConfiguration) cfg;
      return new BernoulliDistributionConfiguration
             {
               Id = master.Id,
               Probability = master.Probability
             };
    }

    public IBoolDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (BernoulliDistributionConfiguration) cfg;
      return new BernoulliDistribution
             {
               Probability = distConfig.Probability
             };
    }

    public IBoolDistribution CreateDistribution()
    {
      return new BernoulliDistribution();
    }
  }
}