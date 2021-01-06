using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class EmpiricalRealDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Empirical"; }
    }

    public string Description
    {
      get { return ""; }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is EmpiricalRealDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new EmpiricalRealDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (EmpiricalRealDistributionConfiguration) cfg;
      var clone = new EmpiricalRealDistributionConfiguration
                  {
                    Id = master.Id
                  };
      foreach (var entry in master.Entries)
      {
        clone.Add(entry);
      }
      return clone;
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (EmpiricalRealDistributionConfiguration) cfg;
      var dist = new EmpiricalRealDistribution();
      foreach (var entry in distConfig.Entries)
      {
        dist.TryAddEntry(entry.Value, entry.Frequency);
      }
      return dist;
    }

    public IRealDistribution CreateDistribution()
    {
      return new EmpiricalRealDistribution();
    }
  }
}