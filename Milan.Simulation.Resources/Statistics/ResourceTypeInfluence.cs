using Milan.Simulation.Statistics;

namespace Milan.Simulation.Resources.Statistics
{
  public class ResourceTypeInfluence
  {
    public ResourceTypeInfluence()
    {
      Value = new ValueAccumulator<double>();
    }

    public IEntity Entity { get; set; }
    public IResourceType ResourceType { get; set; }
    public IInfluence Influence { get; set; }
    public ValueAccumulator<double> Value { get; }
  }
}