#region License

// Copyright (c) 2013 HTW Berlin
// All rights reserved.

#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Emporer.Unit.Factories;
using Milan.JsonStore;
using Milan.Simulation.MaterialAccounting.Factories;
using Milan.Simulation.MaterialAccounting.UI.ViewModels;
using Milan.Simulation.UI.ViewModels;

namespace Milan.Simulation.MaterialAccounting.UI.Factories
{
  [Export(typeof (IMaterialObserverViewModelFactory))]
  internal class MaterialObserverViewModelFactory : IMaterialObserverViewModelFactory
  {
    private readonly IEnumerable<IMaterialObserverFactory> _materialObserverFactories;
    private readonly IStandardUnitFactory _standardUnitFactory;
    private readonly IJsonStore _store;

    [ImportingConstructor]
    public MaterialObserverViewModelFactory([Import] IJsonStore store,
                                            [ImportMany] IEnumerable<IMaterialObserverFactory> materialObserverFactories,
                                            [Import] IStandardUnitFactory standardUnitFactory)
    {
      _store = store;
      _materialObserverFactories = materialObserverFactories;
      _standardUnitFactory = standardUnitFactory;
    }


    public IMaterialObserverViewModel Create(IMaterialObserver observer,
                                             IEnumerable<IProductType> availableProductTypes,
                                             ObservableCollection<string> categories)
    {
      // currently there's only one viewmodel that can handle all different types of material observers
      return new MaterialObserverViewModel(observer,
                                           _store,
                                           _materialObserverFactories,
                                           //todo: use factory
                                           availableProductTypes.Select(p => new ProductTypeViewModel(p))
                                                                .ToArray(),
                                           categories,
                                           _standardUnitFactory.StandardUnits);
    }
  }
}