using Emporer.Math.Distribution;
using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  public abstract class NamedProcessConfiguration : DomainEntity, INamedProcessConfiguration
  {
    private readonly string _name;

    protected NamedProcessConfiguration(string name)
    {
      _name = name;
    }

    public string Name
    {
      get { return _name; }
    }

    [JsonProperty]
    public IDistributionConfiguration Duration
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }
  }
}