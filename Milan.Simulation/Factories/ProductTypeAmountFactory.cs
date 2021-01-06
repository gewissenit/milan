using System.ComponentModel.Composition;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IProductTypeAmountFactory))]
  internal class ProductTypeAmountFactory : IProductTypeAmountFactory
  {
    public IProductTypeAmount Create()
    {
      return new ProductTypeAmount();
    }

    public IProductTypeAmount Duplicate(IProductTypeAmount master)
    {
      var clone = Create();
      clone.ProductType = master.ProductType;
      clone.Amount = master.Amount;
      return clone;
    }
  }
}