namespace Milan.Simulation.Factories
{
  public interface IProductTypeAmountFactory
  {
    IProductTypeAmount Create();
    IProductTypeAmount Duplicate(IProductTypeAmount master);
  }
}