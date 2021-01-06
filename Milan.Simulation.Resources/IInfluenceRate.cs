namespace Milan.Simulation.Resources
{
  public interface IInfluenceRate : IEntity
  {
    IInfluence Influence { get; set; }
    double Value { get; set; }
  }
}