using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IIntDistribution>))]
  internal class EmpiricalIntDistributionFactory : IDistributionFactory<IIntDistribution>
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
      return cfg is EmpiricalIntDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new EmpiricalIntDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (EmpiricalIntDistributionConfiguration) cfg;
      var clone = new EmpiricalIntDistributionConfiguration
                  {
                    Id = master.Id
                  };
      foreach (var entry in master.Entries)
      {
        clone.Add(entry);
      }
      return clone;
    }

    public IIntDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (EmpiricalIntDistributionConfiguration) cfg;
      var dist = new EmpiricalIntDistribution();
      foreach (var entry in distConfig.Entries)
      {
        dist.TryAddEntry(entry.Value, entry.Frequency);
      }
      return dist;
    }

    public IIntDistribution CreateDistribution()
    {
      return new EmpiricalIntDistribution();
    }
  }
}