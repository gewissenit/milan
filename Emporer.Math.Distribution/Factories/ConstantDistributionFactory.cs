using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class ConstantDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Constant"; }
    }

    public string Description
    {
      get
      {
        return "A constant pseudo-distribution returns a single constant predefined value. This distribution is most useful for testing purposes";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is ConstantDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new ConstantDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (ConstantDistributionConfiguration) cfg;
      return new ConstantDistributionConfiguration
             {
               Id = master.Id,
               ConstantValue = master.ConstantValue
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (ConstantDistributionConfiguration) cfg;
      var dist = new ConstantDistribution() as IConstantRealDistribution;
      dist.ConstantValue = distConfig.ConstantValue;
      return dist;
    }

    public IRealDistribution CreateDistribution()
    {
      return new ConstantDistribution();
    }
  }
}