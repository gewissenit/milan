using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ResourceTypeInfluence : DomainEntity, IResourceTypeInfluence
  {
    public ResourceTypeInfluence()
    {
      IncreaseFactor = 1;
      RecoveryRate = 0;
      LowerLimit = 0;
      InitialValue = 0;
      UpperLimit = double.MaxValue;
    }

    public ResourceTypeInfluence(IInfluence influence)
      : this()
    {
      Influence = influence;
    }

    [JsonProperty]
    public IInfluence Influence
    {
      get { return Get<IInfluence>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double IncreaseFactor
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double RecoveryRate
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double InitialValue
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double LowerLimit
    {
      get { return Get<double>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public double UpperLimit
    {
      get { return Get<double>(); }
      set { Set(value); }
    }
  }
}