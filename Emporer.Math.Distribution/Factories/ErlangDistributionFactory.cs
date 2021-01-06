using System.ComponentModel.Composition;

namespace Emporer.Math.Distribution.Factories
{
  [Export(typeof (IDistributionFactory<IRealDistribution>))]
  internal class ErlangDistributionFactory : IDistributionFactory<IRealDistribution>
  {
    public string Name
    {
      get { return "Erlang"; }
    }

    public string Description
    {
      get
      {
        return
          "The Erlang-distribution is a continuos distribution often used in queueing theory. Mean parameter determines the average value, Order parameter is the order of the Erlang distribution and must be >= 1.";
      }
    }

    public bool CanHandle(IDistributionConfiguration cfg)
    {
      return cfg is ErlangDistributionConfiguration;
    }

    public IDistributionConfiguration CreateConfiguration()
    {
      return new ErlangDistributionConfiguration();
    }

    public IDistributionConfiguration DuplicateConfiguration(IDistributionConfiguration cfg)
    {
      var master = (ErlangDistributionConfiguration) cfg;
      return new ErlangDistributionConfiguration
             {
               Id = master.Id,
               Mean = master.Mean,
               Minimum = master.Minimum,
               Order = master.Order
             };
    }

    public IRealDistribution CreateAndConfigureDistribution(IDistributionConfiguration cfg)
    {
      var distConfig = (ErlangDistributionConfiguration) cfg;
      return new ErlangDistribution
             {
               Mean = distConfig.Mean,
               Minimum = distConfig.Minimum,
               Order = distConfig.Order
             };
    }

    public IRealDistribution CreateDistribution()
    {
      return new ErlangDistribution();
    }
  }
}