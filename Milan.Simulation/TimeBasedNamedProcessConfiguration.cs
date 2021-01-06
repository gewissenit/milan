using Emporer.Math.Distribution;
using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class TimeBasedNamedProcessConfiguration : NamedProcessConfiguration
  {
    [JsonProperty]
    public IDistributionConfiguration Occurrence
    {
      get { return Get<IDistributionConfiguration>(); }
      set { Set(value); }
    }

    public TimeBasedNamedProcessConfiguration()
      : base("Time Based")
    {
    }
  }
}