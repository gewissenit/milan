using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Milan.Simulation.Factories;

namespace Milan.Simulation.CostAccounting.Factories
{
  [Export(typeof (IAdditionalEntityDuplicationAction))]
  internal class DuplicateCostAccountingsAction : IAdditionalEntityDuplicationAction
  {
    private readonly IEnumerable<ICostObserverFactory> _costObserverFactories;

    [ImportingConstructor]
    public DuplicateCostAccountingsAction([ImportMany] IEnumerable<ICostObserverFactory> costObserverFactories)
    {
      _costObserverFactories = costObserverFactories;
    }

    public void DuplicateEntity(IModel destinationModel, IEntity original, IEntity clone)
    {
      var model = original.Model;
      foreach (var copy in model.Observers.OfType<ICostObserver>()
                                .Where(ms => ms.Entity == original)
                                .Select(costObserver => _costObserverFactories.Single(cof => cof.CanHandle(costObserver)).Duplicate(costObserver))
                                .ToArray())
      {
        copy.Entity = clone;
        destinationModel.Add(copy);
      }
    }
  }
}