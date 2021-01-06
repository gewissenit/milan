using System.ComponentModel.Composition;
using Milan.Simulation.Factories;

namespace EcoFactory.Components.Factories
{
  [Export(typeof (ITransformationRuleFactory))]
  public class TransformationRuleFactory : ITransformationRuleFactory
  {
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;
    private readonly ITransformationRuleOutputFactory _transformationRuleOutputFactory;

    [ImportingConstructor]
    public TransformationRuleFactory([Import] ITransformationRuleOutputFactory transformationRuleOutputFactory,
                                     [Import] IProductTypeAmountFactory productTypeAmountFactory)
    {
      _transformationRuleOutputFactory = transformationRuleOutputFactory;
      _productTypeAmountFactory = productTypeAmountFactory;
    }

    public ITransformationRule Create()
    {
      var newInstance = new TransformationRule();
      return newInstance;
    }

    public ITransformationRule Duplicate(ITransformationRule master)
    {
      var clone = Create();
      clone.Probability = master.Probability;

      foreach (var productTypeAmount in master.Inputs)
      {
        clone.AddInput(_productTypeAmountFactory.Duplicate(productTypeAmount));
      }

      foreach (var output in master.Outputs)
      {
        clone.AddOutput(_transformationRuleOutputFactory.Duplicate(output));
      }

      return clone;
    }
  }
}