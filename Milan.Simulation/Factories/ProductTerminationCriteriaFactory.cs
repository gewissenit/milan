using System.ComponentModel.Composition;
using System.Linq;
using Milan.Simulation.Observers;

namespace Milan.Simulation.Factories
{
  [Export(typeof (ITerminationCriteriaFactory))]
  [Export(typeof(ISimulationObserverFactory))]
  internal class ProductTerminationCriteriaFactory : ITerminationCriteriaFactory
  {
    private readonly IProductTypeAmountFactory _productTypeAmountFactory;

    [ImportingConstructor]
    public ProductTerminationCriteriaFactory([Import] IProductTypeAmountFactory productTypeAmountFactory)
    {
      _productTypeAmountFactory = productTypeAmountFactory;
    }

    public ITerminationCriteria Create()
    {
      var newInstance = new ProductTerminationCriteria();
      return newInstance;
    }

    public ITerminationCriteria Duplicate(ITerminationCriteria terminationCriteria)
    {
      return Clone(terminationCriteria);
    }

    public bool CanHandle(ISimulationObserver observer)
    {
      return observer.GetType() == typeof (ProductTerminationCriteria);
    }

    public ISimulationObserver CreateSimulationObserver(ISimulationObserver simulationObserver, IExperiment experiment)
    {
      var clone = Clone(simulationObserver);
      clone.Id = simulationObserver.Id;
      clone.Configure(experiment);
      return clone;
    }

    public void Prepare(ISimulationObserver simulationObserver)
    {
      var observer = (ProductTerminationCriteria) simulationObserver;
      var modelProductTypes = observer.Model.Entities.OfType<IProductType>()
                                      .ToArray();
      foreach (var amountForProductType in observer.ProductAmounts)
      {
        var productType = modelProductTypes.Single(e => e.Id == amountForProductType.ProductType.Id);
        amountForProductType.ProductType = productType;
      }
    }

    private ITerminationCriteria Clone(ISimulationObserver simulationObserver)
    {
      var master = (ProductTerminationCriteria) simulationObserver;
      var clone = new ProductTerminationCriteria
                  {
                    HasAndOperator = master.HasAndOperator
                  };

      foreach (var productTypeAmount in master.ProductAmounts)
      {
        clone.Add(_productTypeAmountFactory.Duplicate(productTypeAmount));
      }

      return clone;
    }
  }
}