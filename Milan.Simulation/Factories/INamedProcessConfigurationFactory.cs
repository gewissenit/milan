namespace Milan.Simulation.Factories
{
  public interface INamedProcessConfigurationFactory
  {
    string Name { get; }
    INamedProcessConfiguration Create();
  }
}