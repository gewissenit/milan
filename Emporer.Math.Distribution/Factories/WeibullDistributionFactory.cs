using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class WeibullDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Weibull"; }
    }

    public string Description
    {
      get { return "Weibull distribution using scale and shape parameter. Both values must be nonnegative."; }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is WeibullDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new WeibullDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (WeibullDistributionConfiguration) cfg;
      return new WeibullDistributionConfiguration
             {
               Id = master.Id,
               Scale = master.Scale,
               Shape = master.Shape
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (WeibullDistributionConfiguration) cfg;
      return new WeibullDistribution
             {
               Scale = distConfig.Scale,
               Shape = distConfig.Shape
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new WeibullDistribution();
    }
  }
}