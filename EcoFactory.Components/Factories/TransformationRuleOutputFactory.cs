using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Math.Distribution;
using Emporer.Math.Distribution.Factories;
using Milan.Simulation.Factories;
using Milan.Simulation.Resources.Factories;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (ITransformationRuleOutputFactory))]
  internal class TransformationRuleOutputFactory : ITransformationRuleOutputFactory
  {
    private readonly IEnumerable<IDistributionFactory<IRealDistribution>> _distributionFactories;
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly IResourcePoolResourceTypeAmountFactory _resourcePoolResourceTypeAmountFactory;

    [ImportingConstructor]
    public TransformationRuleOutputFactory([ImportMany] IEnumerable<IDistributionFactory<IRealDistribution>> distributionFactories,
                                           [Import] IProductTypeAmountFactory productTypeAmountFactory,
                                           [Import] IResourcePoolResourceTypeAmountFactory resourcePoolResourceTypeAmountFactory)
    {
      _distributionFactories = distributionFactories;
      _productTypeAmountFactory = productTypeAmountFactory;
      _resourcePoolResourceTypeAmountFactory = resourcePoolResourceTypeAmountFactory;
    }

    public ITransformationRuleOutput Create()
    {
      var newInstance = new TransformationRuleOutput();
      return newInstance;
    }

    public ITransformationRuleOutput Duplicate(ITransformationRuleOutput master)
    {
      var clone = Create();
      clone.Probability = master.Probability;
      if (master.ProcessingDuration != null)
      {
        clone.ProcessingDuration = _distributionFactories.Single(df => df.CanHandle(master.ProcessingDuration))
                                                          .DuplicateConfiguration(master.ProcessingDuration);
      }

      foreach (var productTypeAmount in master.Outputs)
      {
        clone.Add(_productTypeAmountFactory.Duplicate(productTypeAmount));
      }

      foreach (var resource in master.Resources)
      {
        clone.Add(_resourcePoolResourceTypeAmountFactory.Duplicate(resource));
      }
      return clone;
    }
  }
}