namespace Milan.Simulation.Resources.Factories
{
  public interface IResourcePoolResourceTypeAmountFactory
  {
    IResourcePoolResourceTypeAmount Create();
    IResourcePoolResourceTypeAmount Duplicate(IResourcePoolResourceTypeAmount master);
  }
}