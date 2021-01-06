namespace Milan.Simulation.Resources.Factories
{
  public interface IResourceTypeAmountFactory
  {
    IResourceTypeAmount Create();
    IResourceTypeAmount Duplicate(IResourceTypeAmount master);
  }
}