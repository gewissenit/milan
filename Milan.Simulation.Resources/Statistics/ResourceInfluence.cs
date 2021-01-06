using Milan.Simulation.Statistics;

namespace Milan.Simulation.Resources.Statistics
{
  public class ResourceInfluence
  {
    public Resource Resource { get; set; }
    public IInfluence Influence { get; set; }
    public ValueAccumulator<double> Value { get; set; }
  }
}