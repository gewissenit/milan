using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;

namespace Milan.Simulation.Factories
{
  [Export(typeof (IProductTypeDistributionFactory))]
  internal class ProductTypeDistributionFactory : IProductTypeDistributionFactory
  {
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;

    [ImportingConstructor]
    public ProductTypeDistributionFactory([ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories)
    {
      _distributionFactories = distributionFactories;
    }

    public IProductTypeDistribution Create()
    {
      var newInstance = new ProductTypeDistribution();
      return newInstance;
    }

    public IProductTypeDistribution Duplicate(IProductTypeDistribution master)
    {
      return new ProductTypeDistribution
      {
        ProductType = master.ProductType,
        DistributionConfiguration = _distributionFactories.Single(df => df.CanHandle(master.DistributionConfiguration)).DuplicateConfiguration(master.DistributionConfiguration),
      };
    }
  }
}