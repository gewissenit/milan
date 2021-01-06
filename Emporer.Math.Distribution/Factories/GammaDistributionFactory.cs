using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class GammaDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Gamma"; }
    }

    public string Description
    {
      get
      {
        return
          "Gamma distribution using scale and shape parameter. Absolute values of the values entered here will be used. The value of the minimum parameter will be added to all samples.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is GammaDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new GammaDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (GammaDistributionConfiguration) cfg;
      return new GammaDistributionConfiguration
             {
               Id = master.Id,
               Scale = master.Scale,
               Shape = master.Shape,
               Minimum = master.Minimum
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (GammaDistributionConfiguration) cfg;
      return new GammaDistribution
             {
               Scale = distConfig.Scale,
               Minimum = distConfig.Minimum,
               Shape = distConfig.Shape
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new GammaDistribution();
    }
  }
}