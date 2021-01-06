namespace Milan.Simulation.Resources.Factories
{
  public interface IProductTypeSpecificResourceFactory
  {
    IProductTypeSpecificResource Create();
    IProductTypeSpecificResource Duplicate(IProductTypeSpecificResource master);
  }
}