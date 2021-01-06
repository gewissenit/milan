using Newtonsoft.Json;

namespace Milan.Simulation
{
  [JsonObject(MemberSerialization.OptIn)]
  public class AmountBasedNamedProcessConfiguration : NamedProcessConfiguration, INamedProcessConfiguration
  {
    [JsonProperty]
    public int Amount
    {
      get { return Get<int>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public bool PerBatch
    {
      get { return Get<bool>(); }
      set { Set(value); }
    }

    public AmountBasedNamedProcessConfiguration()
      : base("Amount Based")
    {
    }
  }
}