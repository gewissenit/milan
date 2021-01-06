using System.ComponentModel.Composition;

namespace Milan.Simulation.Resources.Factories
{
  [Export(typeof(IProductTypeSpecificResourceFactory))]
  internal class ProductTypeSpecificResourceFactory: IProductTypeSpecificResourceFactory
  {
    public IProductTypeSpecificResource Create()
    {
      return new ProductTypeSpecificResource();
    }

    public IProductTypeSpecificResource Duplicate(IProductTypeSpecificResource master)
    {
     return new ProductTypeSpecificResource()
             {
               ProductType = master.ProductType,
               ResourcePool = master.ResourcePool,
               ResourceType = master.ResourceType,
               Amount = master.Amount
             };
    }
  }
}