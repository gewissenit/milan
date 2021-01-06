using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Milan.Simulation.Factories;

namespace Milan.Simulation.MaterialAccounting.Factories
{
  [Export(typeof (IAdditionalEntityDuplicationAction))]
  internal class DuplicateMaterialAccountingsAction : IAdditionalEntityDuplicationAction
  {
    private readonly IEnumerable<IMaterialObserverFactory> _materialObserverFactories;

    [ImportingConstructor]
    public DuplicateMaterialAccountingsAction([ImportMany] IEnumerable<IMaterialObserverFactory> materialObserverFactories)
    {
      _materialObserverFactories = materialObserverFactories;
    }

    public void DuplicateEntity(IModel destinationModel, IEntity original, IEntity clone)
    {
      var model = original.Model;
      foreach (var copy in model.Observers.OfType<IMaterialObserver>()
                                .Where(ms => ms.Entity == original)
                                .Select(materialObserver => _materialObserverFactories.Single(cof => cof.CanHandle(materialObserver)).Duplicate(materialObserver))
                                .ToArray())
      {
        copy.Entity = clone;
        destinationModel.Add(copy);
      }
    }
  }
}