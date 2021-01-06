using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class InfluenceRate : Entity, IInfluenceRate
  {
    public InfluenceRate()
    {
      Value = 1.0;
    }

    [JsonProperty]
    public IInfluence Influence
    {
      get { return Get<IInfluence>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double Value
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}