namespace Milan.Simulation.Factories
{
  public interface IProductTypeDistributionFactory
  {
    IProductTypeDistribution Create();
    IProductTypeDistribution Duplicate(IProductTypeDistribution master);
  }
}