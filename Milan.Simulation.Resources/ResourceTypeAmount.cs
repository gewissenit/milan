using Milan.JsonStore;
using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class ResourceTypeAmount : DomainEntity, IResourceTypeAmount
  {
    [JsonProperty]
    public IResourceType ResourceType
    {
      get { return Get<IResourceType>(); }
      set { Set(value); }
    }

    [JsonProperty]
    public int Amount
    {
      get { return Get<int>(); }
      set { Set(value); }
    }
  }
}