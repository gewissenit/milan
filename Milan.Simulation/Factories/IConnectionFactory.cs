namespace Milan.Simulation.Factories
{
  public interface IConnectionFactory
  {
    IConnection Create();
    IConnection Duplicate(IConnection connection);
  }
}