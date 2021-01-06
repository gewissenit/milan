using Newtonsoft.Json;

namespace Milan.Simulation.Resources
{
  [JsonObject(MemberSerialization.OptIn)]
  public class Influence : Entity, IInfluence
  {
  }
}