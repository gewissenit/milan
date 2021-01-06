using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class UniformDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Uniform"; }
    }

    public string Description
    {
      get { return "Uniform distribution of values between the From and the To parameter."; }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is UniformDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new UniformDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (UniformDistributionConfiguration) cfg;
      return new UniformDistributionConfiguration
             {
               Id = master.Id,
               LowerBorder = master.LowerBorder,
               UpperBorder = master.UpperBorder
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (UniformDistributionConfiguration) cfg;
      var dist = new UniformDistribution() as IUniformDistribution;
      dist.LowerBorder = distConfig.LowerBorder;
      dist.UpperBorder = distConfig.UpperBorder;
      return dist as IRealDistribution;
    }

    public IRealDistribution CreateDistribution()
    {
      return new UniformDistribution();
    }
  }
}