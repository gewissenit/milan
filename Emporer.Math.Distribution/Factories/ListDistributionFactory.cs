using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class ListDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "List of Values"; }
    }

    public string Description
    {
      get
      {
        return "A list distribution.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is ListDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new ListDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (ListDistributionConfiguration) cfg;
      var clone = new ListDistributionConfiguration
      {
        Id = master.Id,
        IsPeriodic = master.IsPeriodic
      };
      foreach (var value in master.Values)
      {
        clone.Values.Add(value);
      }
      return clone;
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (ListDistributionConfiguration) cfg;
      var dist = new ListDistribution
                 {
                   IsPeriodic = distConfig.IsPeriodic
                 };
      foreach (var value in distConfig.Values)
      {
        dist.AddEntry((double) value);
      }
      return dist;
    }

    public IRealDistribution CreateDistribution()
    {
      return new ListDistribution();
    }
  }
}